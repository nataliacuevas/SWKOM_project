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
       
        private const string QueueName = "post";

        public RabbitMQworker(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
           
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
            var minioRepo = new MinioRepository();

            // Retrieve the file from MinIO
            using var memoryStream = await minioRepo.Get(documentId);

            // Perform OCR processing
            OcrClient ocrClient = new OcrClient(new OcrOptions());
            var ocrContentText = ocrClient.OcrPdf(memoryStream);

            Console.WriteLine($"OCR Processed Content: {ocrContentText}");

            // Initialize Elasticsearch
            var elasticsearchRepo = new ElasticsearchRepository();
            await elasticsearchRepo.InitializeAsync();

            // Index the document in Elasticsearch
            await elasticsearchRepo.IndexDocumentAsync(
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
