using RabbitMQ.Client;

namespace OCRworker.Repositories
{
    public class ConnectionFactoryWrapper : IConnectionFactory
    {
        private readonly ConnectionFactory _connectionFactory;

        public ConnectionFactoryWrapper(string hostName, string virtualHost)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = hostName,
                VirtualHost = virtualHost
            };
        }

        public IConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }
    }
}