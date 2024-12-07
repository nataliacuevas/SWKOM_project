using NPaperless.OCRLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using OCRworker.Repositories;
using OCRworker;
using Elastic.Clients.Elasticsearch;



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
    string documentId = message.Split(" ").Last();
    var minioRepo = new MinioRepository();
    using var memoryStream = await minioRepo.Get(documentId);

    OcrClient ocrClient = new OcrClient(new OcrOptions());
    var ocrContentText = ocrClient.OcrPdf(memoryStream);

    Console.WriteLine($"OCR Processed Content: {ocrContentText}");

    var elasticsearchRepo = new ElasticsearchRepository();
    await elasticsearchRepo.InitializeAsync();
    Console.WriteLine("Elasticsearch index initialized.");
    await elasticsearchRepo.IndexDocumentAsync(Convert.ToInt64(documentId), ocrContentText, DateTime.Now);
    Console.WriteLine("Result sent to ElasticSearch");
}


