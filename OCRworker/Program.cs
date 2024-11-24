// See https://aka.ms/new-console-template for more information
using NPaperless.OCRLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OCRworker.Repositories;



Console.WriteLine("OCR with Tesseract Demo!");
// TODO: implement this in a correct way 
Thread.Sleep(1000*60*2);
Console.WriteLine("Just woke up");
//string filePath = "./docs/TourPlanner_Specification.pdf";

//try
//{
/*
using FileStream fileStream = new FileStream(filePath, FileMode.Open);
using StreamReader reader = new StreamReader(fileStream);
OcrClient ocrClient = new OcrClient(new OcrOptions());

var ocrContentText = ocrClient.OcrPdf(fileStream);
Console.WriteLine( ocrContentText );
*/
// To retrieve message from RabbitMQ

var factory = new ConnectionFactory
{
    HostName = "rabbitmq",
    VirtualHost = "mrRabbit"
};
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: "post",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Received {message}");
    ProcessMessage(message); // Custom task
};

channel.BasicConsume(queue: "post", autoAck: true, consumer: consumer);

while (true)
{
    Thread.Sleep(1000); // Keeps the worker alive
}

async Task ProcessMessage(string message)
{
    string id = message.Split(" ").Last();
    var minioRepo = new MinioRepository();
    using var memoryStream = await minioRepo.Get(id);

   // using FileStream fileStream = new FileStream(filePath, FileMode.Open);

    using StreamReader reader = new StreamReader(memoryStream);
    OcrClient ocrClient = new OcrClient(new OcrOptions());

    var ocrContentText = ocrClient.OcrPdf(memoryStream);
    Console.WriteLine(ocrContentText);

}

//}
//catch (IOException e)
// {
//   Console.WriteLine("An error occurred: " + e.Message);
//}
