
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
   
    public class RabbitMQRepository : IRabbitMQRepository, IDisposable
    {
        private readonly IModel _channel;
        private readonly IConnection _connection;
        private readonly ConnectionFactory _factory;

        public RabbitMQRepository(IConnectionFactory factory)
        {
         
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

        }

        // Implement IDisposable to clean up resources
        public void Dispose()
        {
            _channel?.Dispose(); 
            _connection?.Dispose();
        }
        public void Subscribe(string queue, EventHandler<BasicDeliverEventArgs> subscription)
        {
            _channel.QueueDeclare(queue: queue,
                    durable: true, 
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += subscription;
            _channel.BasicConsume(queue: queue, autoAck: true, consumer: consumer);

        }
        public void SimpleSubscribe(string queue, ProcessDelegate subscription)
        {
            EventHandler<BasicDeliverEventArgs> func = async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"Received {message}");
                await subscription(message);

            };
            Subscribe(queue, func);
        }

        public void Send(string queue, string message)
        {
            //TODO: IMPLEMENT 
        }


    }

}
