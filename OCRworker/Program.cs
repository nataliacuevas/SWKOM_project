using NPaperless.OCRLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OCRworker.Repositories;
using RabbitMQ.Client.Exceptions;



Console.WriteLine("OCR with Tesseract Demo!");



while (true)
{
    try
    {
        var rabbit = new RabbitMQRepository();
        rabbit.SimpleSubscribe("post", ProcessMessage);
        break;
       
    }
    catch (BrokerUnreachableException)
    {
        Console.WriteLine("retrying connection");
        Thread.Sleep(1000); // Keeps the worker alive

    }
}
Console.WriteLine("starting to listen");
while (true);

async Task ProcessMessage(string message)
{
    string id = message.Split(" ").Last();
    var minioRepo = new MinioRepository();
    using var memoryStream = await minioRepo.Get(id); //fetch

    using StreamReader reader = new StreamReader(memoryStream);
    OcrClient ocrClient = new OcrClient(new OcrOptions());

    var ocrContentText = ocrClient.OcrPdf(memoryStream); //tesseract
    Console.WriteLine(ocrContentText);

}

