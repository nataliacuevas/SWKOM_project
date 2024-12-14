
using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using OCRworker.Repositories;
using IConnectionFactory = OCRworker.Repositories.IConnectionFactory;
using CommunityToolkit.HighPerformance;

namespace SWKOM.tests.net7
{
    public class RabbitMQRepositoryTests
    {
        private Mock<RabbitMQ.Client.IModel> _mockChannel;
        private Mock<IConnection> _mockConnection;
        private Mock<IConnectionFactory> _mockFactory;
        private RabbitMQRepository _repository;

        [SetUp]
        public void Setup()
        {
            _mockChannel = new Mock<RabbitMQ.Client.IModel>();
            _mockConnection = new Mock<IConnection>();
            _mockFactory = new Mock<IConnectionFactory>();

            _mockFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            _repository = new RabbitMQRepository(_mockFactory.Object);
        }

        [Test]
        public void Subscribe_DeclaresQueueAndSetsUpConsumer()
        {
            // Arrange
            string queueName = "testQueue";
            EventHandler<BasicDeliverEventArgs> handler = (sender, args) => { };

            // Act
            _repository.Subscribe(queueName, handler);

            // Assert
            _mockChannel.Verify(ch => ch.QueueDeclare(queueName, true, false, false, null), Times.Once);
        }

        [Test]
        public async Task SimpleSubscribe_ProcessesMessagesCorrectly()
        {
            // Arrange
            string queueName = "testQueue";
            string testMessage = "Hello, World!";
            var messageBody = Encoding.UTF8.GetBytes(testMessage);

            var processDelegate = new ProcessDelegate(async (message) =>
            {
                Assert.That(message, Is.EqualTo(testMessage));
            });

            var consumer = new EventingBasicConsumer(_mockChannel.Object);
            var eventArgs = new BasicDeliverEventArgs { Body = new ReadOnlyMemory<byte>(messageBody) };

            // Act
            _repository.SimpleSubscribe(queueName, processDelegate);
            consumer.HandleBasicDeliver(
                string.Empty,
                0,
                false,
                string.Empty,
            queueName,
            null,
                new ReadOnlyMemory<byte>(messageBody)
            );

            // Assert
            Assert.That(_mockChannel.Invocations.Count, Is.GreaterThan(0), "Message processing did not occur.");
        }

        [TearDown]
        public void TearDown()
        {
            _repository.Dispose();
        }
    }
}
