using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NPaperless.OCRLibrary;
using OCRworker.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCRworker
{
    public class RabbitMQworker : BackgroundService
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly IMinioRepository _minioRepository;
        private readonly IOcrClient _ocrClient;
        private readonly IElasticsearchRepository _elasticsearchRepository;


        private const string QueueName = "post";

        public RabbitMQworker(
            IConnectionFactory connectionFactory,
            IMinioRepository minioRepository,
            IOcrClient ocrClient,
            IElasticsearchRepository elasticsearchRepository)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            _minioRepository = minioRepository ?? throw new ArgumentNullException(nameof(minioRepository));
            _ocrClient = ocrClient ?? throw new ArgumentNullException(nameof(ocrClient));
            _elasticsearchRepository = elasticsearchRepository ?? throw new ArgumentNullException(nameof(elasticsearchRepository));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
           

            try
            {
                using var connection = _connectionFactory.CreateConnection();
                using var channel = connection.CreateModel();

                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                       
                        await ProcessMessageAsync(message);
                    }
                    catch (Exception ex)
                    {
                       
                    }
                };

                channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
               
            }

           
        }


        async Task ProcessMessageAsync(string message)
        {
            string documentId = message.Split(" ").Last();


            // Retrieve the file from MinIO
            using var memoryStream = await _minioRepository.Get(documentId);

            // Perform OCR processing
            
            var ocrContentText = _ocrClient.OcrPdf(memoryStream);

            Console.WriteLine($"OCR Processed Content: {ocrContentText}");

            // Initialize Elasticsearch
           
            await _elasticsearchRepository.InitializeAsync();

            // Index the document in Elasticsearch
            await _elasticsearchRepository.IndexDocumentAsync(
                id: long.Parse(documentId),
                name: $"Document_{documentId}",
                content: ocrContentText,
                file: memoryStream.ToArray(),
                timestamp: DateTime.UtcNow
            );

            Console.WriteLine($"Document {documentId} indexed successfully in Elasticsearch.");
        }


    }
}
