using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using sws.DAL.Repositories;
using sws.DAL.Entities;
using log4net;
using sws.DAL;

namespace sws.BLL
{
    public class DocumentSearchService : IDocumentSearchService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DocumentSearchService));
        private readonly IElasticsearchRepository _elasticsearchRepository;
        private readonly IDocumentRepository _documentRepository;

        public DocumentSearchService(IElasticsearchRepository elasticsearchRepository, IDocumentRepository documentRepository)
        {
            _elasticsearchRepository = elasticsearchRepository;
            _documentRepository = documentRepository;
        }

        public async Task<List<DocumentSearchResult>> SearchDocumentsAsync(string query)
        {
            log.Info($"Initiating search for documents with query: '{query}'");

            try
            {
                // Step 1: Search Elasticsearch for document IDs
                log.Debug("Searching Elasticsearch for matching document IDs.");
                var documentIds = await _elasticsearchRepository.SearchQueryInDocumentContent(query);

                if (!documentIds.Any())
                {
                    log.Info("No documents found for the given query.");
                    return new List<DocumentSearchResult>();
                }

                // Step 2: Fetch metadata from the database
                log.Debug($"Fetching metadata for document IDs: {string.Join(", ", documentIds)}");
                var documents = await _documentRepository.GetDocumentsByIdsAsync(documentIds);

                // Step 3: Combine results
                log.Info($"Successfully retrieved {documents.Count} documents for the query.");
                return documents.Select(doc => new DocumentSearchResult
                {
                    Id = doc.Id,
                    Name = doc.Name,
                }).ToList();
            }
            catch (DataAccessException ex)
            {
                log.Error("Data access error occurred during document search.", ex);
                throw new BusinessLogicException("Error searching documents in the business logic layer.", ex);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error occurred during document search.", ex);
                throw new BusinessLogicException("Unexpected error occurred while searching documents.", ex);
            }
        }
    }

    public class DocumentSearchResult
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
