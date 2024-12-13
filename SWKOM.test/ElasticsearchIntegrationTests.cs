using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using NUnit.Framework;
using OCRworker.Repositories;

namespace SWKOM.test
{
    [TestFixture]
    public class ElasticsearchIntegrationTests
    {
        private ElasticsearchRepository _repository;
        private ElasticsearchClient _client;

        [SetUp]
        public async Task SetUp()
        {
            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("ocr-results");
            _client = new ElasticsearchClient(settings);

            _repository = new ElasticsearchRepository(_client);

            // Ensure a clean slate by deleting the index if it exists
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            if (existsResponse.Exists)
            {
                await _client.Indices.DeleteAsync("ocr-results");
            }
        }

        [Test]
        public async Task InitializeAsync_CreatesIndexIfNotExists()
        {
            // Act
            await _repository.InitializeAsync();

            // Assert
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            Assert.That(existsResponse.Exists, Is.True, "Index should have been created.");
        }

        [Test]
        public async Task IndexDocumentAsync_IndexesDocumentSuccessfully()
        {
            // Arrange
            long documentId = 1;
            string content = "This is a test document.";
            DateTime timestamp = DateTime.UtcNow;

            await _repository.InitializeAsync();

            // Act
            await _repository.IndexDocumentAsync(documentId, content, timestamp);

            // Assert
            var response = await _client.GetAsync<ElasticsearchRepository.ElasticsearchDocument>(
                new Elastic.Clients.Elasticsearch.GetRequest("ocr-results", documentId.ToString()));
            Assert.That(response.Source, Is.Not.Null, "Document should be retrievable.");
            Assert.That(response.Source.Id, Is.EqualTo(documentId), "Document ID should match.");
            Assert.That(response.Source.Content, Is.EqualTo(content), "Document content should match.");
        }

       
    }
}
