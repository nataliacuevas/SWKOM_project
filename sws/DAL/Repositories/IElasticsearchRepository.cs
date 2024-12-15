using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace sws.DAL.Repositories
{
    public interface IElasticsearchRepository
    {

        Task<List<long>> SearchQueryInDocumentContent(string query);

    }
}
