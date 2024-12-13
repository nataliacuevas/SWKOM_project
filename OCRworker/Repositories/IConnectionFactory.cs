using RabbitMQ.Client;

namespace OCRworker.Repositories
{
    public interface IConnectionFactory
    {
        IConnection CreateConnection();
    }
}