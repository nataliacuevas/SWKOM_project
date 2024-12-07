
using Moq;
using NPaperless.OCRLibrary;
using NUnit.Framework;
using OCRworker;
using OCRworker.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace SWKOM.test
{
    [TestFixture]
    public class RabbitMQworkerTests : IDisposable
    {
        private Mock<IConnectionFactory> _mockConnectionFactory;
        private Mock<IConnection> _mockConnection;
        private Mock<IModel> _mockChannel;

        private Mock<IMinioRepository> _mockMinioRepository;
        private Mock<IOcrClient> _mockOcrClient;
        private Mock<IElasticsearchRepository> _mockElasticsearchRepository;

        private RabbitMQworker _worker;
        private IBasicConsumer _capturedConsumer;

        [SetUp]
        public void Setup()
        {
            _mockConnectionFactory = new Mock<IConnectionFactory>();
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateModel()).Returns(_mockChannel.Object);

            _mockChannel
                .Setup(c => c.QueueDeclare("post", true, false, false, null))
                .Returns((QueueDeclareOk)null);

            _mockChannel
                .Setup(c => c.BasicConsume(
                    "post",
                    true,
                    It.IsAny<string>(),
                    false,
                    false,
                    null,
                    It.IsAny<IBasicConsumer>()))
                .Callback((string queue, bool autoAck, string consumerTag, bool noLocal, bool exclusive,
                           System.Collections.Generic.IDictionary<string, object> arguments, IBasicConsumer consumer) =>
                {
                    _capturedConsumer = consumer;
                })
                .Returns("consumerTag");

            _mockMinioRepository = new Mock<IMinioRepository>();
            _mockOcrClient = new Mock<IOcrClient>();
            _mockElasticsearchRepository = new Mock<IElasticsearchRepository>();

            // Setup return values for mocks
            var testStream = new MemoryStream(Encoding.UTF8.GetBytes("PDF content"));
            _mockMinioRepository.Setup(m => m.Get(It.IsAny<string>())).ReturnsAsync(testStream);
            _mockOcrClient.Setup(o => o.OcrPdf(It.IsAny<Stream>())).Returns("Extracted OCR text");
            _mockElasticsearchRepository.Setup(e => e.InitializeAsync()).Returns(Task.CompletedTask);
            _mockElasticsearchRepository.Setup(e => e.IndexDocumentAsync(
                It.IsAny<long>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            _worker = new RabbitMQworker(
                _mockConnectionFactory.Object,
                _mockMinioRepository.Object,
                _mockOcrClient.Object,
                _mockElasticsearchRepository.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            (_worker as IDisposable)?.Dispose();
            _mockConnectionFactory = null;
            _mockConnection = null;
            _mockChannel = null;
            _mockMinioRepository = null;
            _mockOcrClient = null;
            _mockElasticsearchRepository = null;
        }

        public void Dispose()
        {
            TearDown();
        }

        [Test]
        public async Task ExecuteAsync_WhenMessageArrives_ShouldProcessMessageAsync()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var workerTask = _worker.StartAsync(cts.Token);

            // Wait for the worker to start and set up the consumer
            await Task.Delay(500);

            // Simulate message delivery
            if (_capturedConsumer is EventingBasicConsumer eventingConsumer)
            {
                var body = Encoding.UTF8.GetBytes("Test message 42");
                var ea = new BasicDeliverEventArgs
                {
                    Body = new ReadOnlyMemory<byte>(body)
                };

                // Trigger the Received event
                eventingConsumer.HandleBasicDeliver(
                    consumerTag: "consumerTag",
                    deliveryTag: 1,
                    redelivered: false,
                    exchange: "",
                    routingKey: "post",
                    properties: null,
                    body: ea.Body
                );
            }

            // Wait for message processing
            await Task.Delay(500);

            // Stop the worker
            cts.Cancel();
            await workerTask;

            // Verify interactions
            _mockMinioRepository.Verify(m => m.Get("42"), Times.Once);
            _mockOcrClient.Verify(o => o.OcrPdf(It.IsAny<Stream>()), Times.Once);
            _mockElasticsearchRepository.Verify(e => e.InitializeAsync(), Times.Once);
            _mockElasticsearchRepository.Verify(e => e.IndexDocumentAsync(
                42,
                "Document_42",
                "Extracted OCR text",
                It.IsAny<byte[]>(),
                It.IsAny<DateTime>()), Times.Once);
        }

        [Test]
        public async Task ExecuteAsync_ShouldStopOnCancellation()
        {
            var cts = new CancellationTokenSource();
            var task = _worker.StartAsync(cts.Token);
            cts.Cancel();

            Assert.DoesNotThrowAsync(async () => await task);
        }
    }
}
