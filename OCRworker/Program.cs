using NPaperless.OCRLibrary;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System.Text;
using OCRworker.Repositories;
using OCRworker;
using Elastic.Clients.Elasticsearch;



Console.WriteLine("OCR with Tesseract Demo!");
var factory = new ConnectionFactoryWrapper("rabbitmq", "mrRabbit");

// retries connection until rabbitmq is ready
while (true)
{
    try
    {
        var rabbit = new RabbitMQRepository(factory);
        rabbit.SimpleSubscribe("post", ProcessMessage);
        break;

    }
    catch (BrokerUnreachableException)
    {
        Console.WriteLine("retrying connection");
        Thread.Sleep(1000); // wait 1 sec to retry connection

    }
}
Console.WriteLine("starting to listen");

while (true) ; // Keeps the worker alive while waiting for messages in queue


async Task ProcessMessage(string message)
{
    string documentId = message.Split(" ").Last();
    var minioRepo = new MinioRepository();
    // get file from minIO as stream
    using var memoryStream = await minioRepo.Get(documentId);

    OcrClient ocrClient = new OcrClient(new OcrOptions());
    var ocrContentText = ocrClient.OcrPdf(memoryStream); //run tesseract on text 

    Console.WriteLine($"OCR Processed Content: {ocrContentText}");

    var elasticsearchRepo = new ElasticsearchRepository();
    // send ocr'ed text to elasticSearch
    await elasticsearchRepo.InitializeIndexAsync();
    Console.WriteLine("Elasticsearch index initialized.");
    await elasticsearchRepo.IndexDocumentAsync(Convert.ToInt64(documentId), ocrContentText, DateTime.Now);
    Console.WriteLine("Result sent to ElasticSearch");
}


