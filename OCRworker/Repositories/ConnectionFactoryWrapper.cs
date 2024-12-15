using Elastic.Clients.Elasticsearch.Graph;
using Elastic.Clients.Elasticsearch.TransformManagement;
using RabbitMQ.Client;

namespace OCRworker.Repositories
{
   // Wrapper for the RabbitMQ ConnectionFactory to create RabbitMQ connections with specified settings.
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
        // Creates a RabbitMQ connection using the configured ConnectionFactory.
        public IConnection CreateConnection()
        {
            return _connectionFactory.CreateConnection();
        }
    }
}