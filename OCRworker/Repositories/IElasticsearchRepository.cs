using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
    public interface IElasticsearchRepository
    {
       
        Task InitializeIndexAsync();
        Task IndexDocumentAsync(long id, string content, DateTime timestamp);


    }
}
