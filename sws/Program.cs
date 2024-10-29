using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using sws.DAL;
using sws.BLL;
using sws.DAL.Repositories;
using sws.BLL.Mappers;
using Microsoft.CodeAnalysis.Elfie.Serialization;

var builder = WebApplication.CreateBuilder(args);

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("/sws/log4net.config"));

// Create a test log entry to verify logging configuration
ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application has started, and logging is configured correctly.");


//register DB
builder.Services.AddDbContext<UploadDocumentContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("DocumentContext")));
// Register Automapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
// Register Repositories
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
// Register Businesslayer 
builder.Services.AddScoped<IDocumentLogic, DocumentLogic>();

// Add services to the containers
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
