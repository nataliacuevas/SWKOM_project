using Elastic.Clients.Elasticsearch;
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
            var settings = new ElasticsearchClientSettings(new Uri("http://localhost:9200"))
                .DefaultIndex("ocr-results");
            _client = new ElasticsearchClient(settings);
        }

        public class ElasticsearchDocument
        {
            public long Id { get; set; }
            public string? Name { get; set; }
            public string? Content { get; set; }
            public byte[]? File { get; set; }
            public DateTime Timestamp { get; set; }
        }

        public async Task InitializeAsync()
        {
            var existsResponse = await _client.Indices.ExistsAsync("ocr-results");
            if (!existsResponse.Exists)
            {
                var createIndexResponse = await _client.Indices.CreateAsync("ocr-results", c => c
                    .Mappings(m => m
                        .Properties<ElasticsearchDocument>(p => p
                            .Keyword(k => k.Id)
                            .Text(t => t.Name)
                            .Text(t => t.Content)
                            .Binary(b => b.File)
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

        public async Task IndexDocumentAsync(long id, string name, string content, byte[] file, DateTime timestamp)
        {
            var document = new ElasticsearchDocument
            {
                Id = id,
                Name = name,
                Content = content,
                File = file,
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
