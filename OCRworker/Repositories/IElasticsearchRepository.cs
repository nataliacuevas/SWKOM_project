using Elastic.Clients.Elasticsearch;
using System;
using System.Threading.Tasks;

namespace OCRworker.Repositories
{
    public interface IElasticsearchRepository
    {
       
        Task InitializeAsync();
        Task IndexDocumentAsync(long id, string name, string content, byte[] file, DateTime timestamp);


    }
}
