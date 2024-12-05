using NPaperless.OCRLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using OCRworker.Repositories;
using OCRworker;



Console.WriteLine("OCR with Tesseract Demo!");

// the container crashes if it tries to subscribe before rabbitMQ is ready
// TODO: implement this in a correct way 
int twoMinutes = 1000 * 60 * 2; //[miliseconds]
Thread.Sleep(twoMinutes);
Console.WriteLine("Just woke up");

using var rabbit = new RabbitMQRepository();
rabbit.SimpleSubscribe("post", ProcessMessage);

while (true)
{
    Thread.Sleep(1000); // Keeps the worker alive
}

async Task ProcessMessage(string message)
{
    string documentId = message.Split(" ").Last();
    var minioRepo = new MinioRepository();
    using var memoryStream = await minioRepo.Get(documentId);

    OcrClient ocrClient = new OcrClient(new OcrOptions());
    var ocrContentText = ocrClient.OcrPdf(memoryStream);

    Console.WriteLine($"OCR Processed Content: {ocrContentText}");

    var elasticsearchRepo = new ElasticsearchRepository();
    await elasticsearchRepo.InitializeAsync();
    Console.WriteLine("Elasticsearch index initialized.");

}


