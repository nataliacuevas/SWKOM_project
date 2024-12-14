using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace sws.DAL.Repositories
{
    public interface IElasticsearchRepository
    {

        Task InitializeAsync();
        Task IndexDocumentAsync(long id, string content, DateTime timestamp);
        Task<List<long>> SearchQueryInDocumentContent(string query);

    }
}
