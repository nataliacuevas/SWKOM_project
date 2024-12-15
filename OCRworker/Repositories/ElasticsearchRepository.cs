﻿using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
    public class ElasticsearchRepository : IElasticsearchRepository
    {
        private readonly ElasticsearchClient _client;
        public ElasticsearchRepository(ElasticsearchClient client)
        {
            _client = client;
        }


        public ElasticsearchRepository()
        {
            var settings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200"))
                .DefaultIndex("ocr-results"); //TODO: maybe not necessary? 
            _client = new ElasticsearchClient(settings);

        }
  
        // Internal representation of a document stored in Elasticsearch
        public class ElasticsearchDocument
        {
            public long Id { get; set; }
            public string? Content { get; set; }
            public DateTime Timestamp { get; set; }
        }

        //ensures ocr-results index is present in elasticSearch, creating if necessary
        public async Task InitializeIndexAsync()
        {
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            if (!existsResponse.Exists)
            {
                var createIndexResponse = await _client.Indices.CreateAsync("ocr-results", c => c
                    .Mappings(m => m
                        .Properties<ElasticsearchDocument>(p => p
                            .LongNumber(k => k.Id)
                            .Text(t => t.Content)
                            .Date(d => d.Timestamp)
                        )
                    )
                );

                if (!createIndexResponse.IsValidResponse)
                {
                    throw new Exception($"Failed to create Elasticsearch index: {createIndexResponse.ElasticsearchServerError}");
                }
            }
        }
        // Indexes a document in Elasticsearch with the specified ID, content, and timestamp.
        public async Task IndexDocumentAsync(long id, string content, DateTime timestamp)
        {
            var document = new ElasticsearchDocument
            {
                Id = id,
                Content = content,
                Timestamp = timestamp
            };

            var response = await _client.IndexAsync(document, i => i.Index("ocr-results").Id(id.ToString()));
            if (!response.IsValidResponse)
            {
                throw new Exception($"Failed to index document {id}: {response.ElasticsearchServerError}");
            }
        }

    }
}
