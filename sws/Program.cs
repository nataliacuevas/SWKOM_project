using Microsoft.EntityFrameworkCore;
using sws.DAL;
using sws.BLL;
using sws.DAL.Repositories;
using sws.BLL.Mappers;
using System.Reflection;
using log4net;
using log4net.Config;
using System.IO;
using Microsoft.CodeAnalysis.Elfie.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

// Create a test log entry to verify logging configuration
ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application has started, and logging is configured correctly.");

//register DB
builder.Services.AddDbContext<UploadDocumentContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DocumentContext")));
// Register Automapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
// Register Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IMinioRepository, MinioRepository>();
// Register Businesslayer 
builder.Services.AddScoped<IDocumentLogic, DocumentLogic>();
// Register Businesslayer 
builder.Services.AddScoped<IUploadDocumentContext, UploadDocumentContext>();

// Add services to the containers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Set the comments path for the Swagger JSON and UI.
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

// register logging provider
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
