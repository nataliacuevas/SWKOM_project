using OCRService;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddSingleton<OCRWorker>();
var app = builder.Build();

// Run the worker

var worker = new OCRWorker();
worker.Start();
