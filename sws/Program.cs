using Microsoft.EntityFrameworkCore;
using sws.DAL;
using sws.BLL;
using sws.DAL.Repositories;
using sws.BLL.Mappers;

var builder = WebApplication.CreateBuilder(args);

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
