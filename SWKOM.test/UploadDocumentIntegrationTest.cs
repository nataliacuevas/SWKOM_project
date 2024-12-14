
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using sws.DAL;
using sws.DAL.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using sws;
using System.Threading.Tasks;

namespace SWKOM.test
{
    [TestFixture]
    public class DocumentUploadIntegrationTest
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // Replace the database context with an in-memory database
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<UploadDocumentContext>));
                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<UploadDocumentContext>(options =>
                            options.UseInMemoryDatabase("TestDatabase"));

                        // Ensure the database is created
                        using var scope = services.BuildServiceProvider().CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<UploadDocumentContext>();
                        db.Database.EnsureCreated();
                    });
                });

            _client = _factory.CreateClient();
        }

        [Test]
        public async Task UploadDocument_ShouldSaveToDatabase()
        {
            // Arrange
            var formData = new MultipartFormDataContent
            {
                { new StringContent("TestDocument"), "Name" },
                { new ByteArrayContent(Encoding.UTF8.GetBytes("Test file content")), "File", "TestDocument.txt" }
            };

            // Act
            var response = await _client.PostAsync("/api/UploadDocument", formData);

            // Assert
            Assert.That(response.IsSuccessStatusCode, Is.True, "The API did not return a success status code.");

            // Validate database state
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UploadDocumentContext>();
            var uploadedDoc = await db.UploadedDocuments.FirstOrDefaultAsync(d => d.Name == "TestDocument");

            Assert.That(uploadedDoc, Is.Not.Null, "The document was not saved to the database.");
            Assert.That(uploadedDoc.Name, Is.EqualTo("TestDocument"), "The document name does not match.");
        }

        [TearDown]
        public void TearDown()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<UploadDocumentContext>();
            db.UploadedDocuments.RemoveRange(db.UploadedDocuments);
            db.SaveChanges();

            _client.Dispose();
            _factory.Dispose();
        }
    }
}