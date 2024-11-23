using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Tesseract;

namespace OCRService
{
    public class OCRWorker
    {
        private const string InputQueue = "ocr_queue";
        private const string OutputQueue = "ocr_result_queue";

        public void Start()
        {
            var factory = new ConnectionFactory { HostName = "rabbitmq" };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: InputQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                Console.WriteLine($"Received: {message}");

                var ocrResult = PerformOCR(message);
                SendResult(channel, ocrResult);
            };

            channel.BasicConsume(queue: InputQueue, autoAck: true, consumer: consumer);

            Console.WriteLine("Waiting for messages...");
            Console.ReadLine();
        }

        private string PerformOCR(string filePath)
        {
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            using var img = Pix.LoadFromFile(filePath);
            using var page = engine.Process(img);

            return page.GetText();
        }
        /*
        private void SendResult(IModel channel, string result)
        {
            var body = Encoding.UTF8.GetBytes(result);
            channel.BasicPublish(exchange: "", routingKey: OutputQueue, basicProperties: null, body: body);
            Console.WriteLine($"Sent: {result}");
        }
        */
    }
}
