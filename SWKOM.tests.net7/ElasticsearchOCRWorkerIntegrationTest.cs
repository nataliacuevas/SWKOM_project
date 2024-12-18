﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.IndexManagement;
using NUnit.Framework;
using OCRworker.Repositories;
using Testcontainers.Elasticsearch;

namespace SWKOM.tests.net7
{
    [TestFixture]
    public class ElasticsearchIntegrationTests
    {
        private ElasticsearchRepository _repository;
        private ElasticsearchClient _client; // Elasticsearch client for direct API interactions
        private ElasticsearchContainer _elasticsearchContainer;  //Testcontainer 

        [SetUp]
        public async Task SetUp()
        {
            //Initialize an Elasticsearch container using Testcontainers
            _elasticsearchContainer = new ElasticsearchBuilder()
                .WithImage("docker.elastic.co/elasticsearch/elasticsearch:8.10.1") //version 
                .WithEnvironment("discovery.type", "single-node")
                .WithEnvironment("xpack.security.enabled", "false")//security disabling for easier testing
                .WithPortBinding(9200, 9200)
                .Build();

            await _elasticsearchContainer.StartAsync();

            //configures elasticsearch client with default index
            string url = "http://localhost:9200";
            var settings = new ElasticsearchClientSettings(new Uri(url))
                .DefaultIndex("ocr-results");
            _client = new ElasticsearchClient(settings);

            //repository being tested
            _repository = new ElasticsearchRepository(_client);

            // Ensure a clean slate by deleting the index if it exists
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            if (existsResponse.Exists)
            {
                await _client.Indices.DeleteAsync("ocr-results");
            }
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_elasticsearchContainer != null)
            {
                await _elasticsearchContainer.DisposeAsync();
            }
        }
        //tests if repository can create the ocr-results index
        [Test]
        public async Task InitializeAsync_CreatesIndexIfNotExists()
        {
            // Act: Call repositorys method to initialize index
            await _repository.InitializeIndexAsync();

            // Assert: verify index was created
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            Assert.That(existsResponse.Exists, Is.True, "Index should have been created.");
        }
        //tests that a document can be indexed correctly in elasticsearch
        [Test]
        public async Task IndexDocumentAsync_IndexesDocumentSuccessfully()
        {
            // Arrange: set up document to be indexed
            long documentId = 1;
            string content = "This is a test document.";
            DateTime timestamp = DateTime.UtcNow;
            //ensure index exists
            await _repository.InitializeIndexAsync();

            // Act
            await _repository.IndexDocumentAsync(documentId, content, timestamp);

            // Assert: retrieve document from elasticsearch and verify content
            var response = await _client.GetAsync<ElasticsearchRepository.ElasticsearchDocument>(
                new Elastic.Clients.Elasticsearch.GetRequest("ocr-results", documentId.ToString()));
            Assert.That(response.Source, Is.Not.Null, "Document should be retrievable.");
            Assert.That(response.Source.Id, Is.EqualTo(documentId), "Document ID should match.");
            Assert.That(response.Source.Content, Is.EqualTo(content), "Document content should match.");
        }


    }
}