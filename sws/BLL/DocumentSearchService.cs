using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sws.DAL.Repositories;
using sws.DAL.Entities;
using sws.SL.DTOs;

namespace sws.BLL
{
    public class DocumentSearchService : IDocumentSearchService
    {
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly IDocumentRepository _documentRepository;

        public DocumentSearchService(IElasticsearchRepository elasticsearchRepository, IDocumentRepository documentRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
            _documentRepository = documentRepository;
        }

        public async Task<List<DocumentSearchDTO>> SearchDocumentsAsync(string query)
        {
            // Step 1: Search Elasticsearch for document IDs
            var documentIds = await _elasticsearchRepository.SearchQueryInDocumentContent(query);

            if (documentIds.Count == 0)
            {
                return new List<DocumentSearchDTO>();
            }

            // Step 2: Fetch metadata from the database
            var documents = await _documentRepository.GetDocumentsByIdsAsync(documentIds);

            // Step 3: Combine results
            return documents.Select(doc => new DocumentSearchDTO
            {
                Id = doc.Id,
                Name = doc.Name,


            }).ToList();
        }
    }
}
