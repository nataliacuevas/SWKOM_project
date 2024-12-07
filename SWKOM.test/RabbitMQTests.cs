using Moq;
using NUnit.Framework;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OCRworker;
using Microsoft.Build.Framework;
/*
namespace SWKOM.test
{
    [TestFixture]
    public class RabbitMQTests : IDisposable
    {
        private Mock<IConnectionFactory> _mockConnectionFactory;
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;
       
        private RabbitMQworker _worker;

        [SetUp]
        public void Setup()
        {
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();
           
            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            // Injecting mocked dependencies
            _worker = new RabbitMQworker(_mockConnectionFactory.Object);
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of worker if it implements IDisposable
            (_worker as IDisposable)?.Dispose();
            _mockConnectionFactory = null;
            _mockConnection = null;
            _mockChannel = null;
          
        }

        public void Dispose()
        {
            TearDown();
        }

        [Test]
        public async Task ExecuteAsync_ShouldDeclareQueueAndConsumeMessages()
        {
            // Arrange
            var stoppingToken = new CancellationTokenSource();
            _mockChannel.Setup(c => c.QueueDeclare("post", false, false, false, null));
            _mockChannel.Setup(c => c.BasicConsume(
                "post",
                true,
                It.IsAny<string>(),
                false,
                false,
                null,
                It.IsAny<IBasicConsumer>()));

            // Act
            var workerTask = Task.Run(() => _worker.StartAsync(stoppingToken.Token));

            // Allow the worker to run briefly
            await Task.Delay(500);
            stoppingToken.Cancel();

            // Assert
            _mockChannel.Verify(c => c.QueueDeclare("post", false, false, false, null), Times.Once);
            _mockChannel.Verify(c => c.BasicConsume(
                "post",
                true,
                It.IsAny<string>(),
                false,
                false,
                null,
                It.IsAny<IBasicConsumer>()), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_ShouldStopWhenCancellationTokenIsCancelled()
        {
            // Arrange
            var stoppingTokenSource = new CancellationTokenSource();

            // Act
            var workerTask = Task.Run(() => _worker.StartAsync(stoppingTokenSource.Token));
            stoppingTokenSource.Cancel();

            // Assert
            Assert.DoesNotThrowAsync(async () => await workerTask);
        }
       

    }
}

*/
