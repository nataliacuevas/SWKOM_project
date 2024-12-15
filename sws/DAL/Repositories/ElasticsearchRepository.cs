using Elastic.Clients.Elasticsearch;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sws.DAL.Repositories
{
    public class ElasticsearchRepository : IElasticsearchRepository
    {
        private readonly ElasticsearchClient _client;
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticsearchClient));


        public ElasticsearchRepository()
        {
            var settings = new ElasticsearchClientSettings(new Uri("http://elasticsearch:9200"))
                .DefaultIndex("ocr-results");
            _client = new ElasticsearchClient(settings);

        }

        public class ElasticsearchDocument
        {
            public long Id { get; set; }
            public string? Content { get; set; }
            public DateTime Timestamp { get; set; }
        }

        //returns matched IDs 
        public async Task<List<long>> SearchQueryInDocumentContent(string query)
        {
            try
            {
                //searches content field for matches to a query string
                _log.Info($"Forwarding to Elasticsearch query: {query}");
                var response = await _client.SearchAsync<ElasticsearchDocument>(s => s
                    .Query(q => q
                        .Match(m => m
                            .Field(f => f.Content)
                            .Query(query)
                        )
                    )
                );

                if (!response.IsValidResponse)
                {
                    throw new DataAccessException($"Search failed: {response.ElasticsearchServerError}");
                }
                // Extract IDs from search results
                return response.Hits.Select(h => h.Source.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("Error performing search in Elasticsearch.", ex);
            }
        }

    }
}
