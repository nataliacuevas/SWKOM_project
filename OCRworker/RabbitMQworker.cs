using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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

        private Task ProcessMessageAsync(string message)
        {
            // Simulate business logic or external processing
           
            return Task.CompletedTask;
        }
    }
}
