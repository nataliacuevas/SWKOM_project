using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sws.SL.DTOs;
using sws.BLL;
using System.Collections.Generic;

namespace sws.SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticsearchController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ElasticsearchController));
        private readonly IDocumentSearchService _documentSearchService;
        public ElasticsearchController(IDocumentSearchService documentSearchService) 
        {
            _documentSearchService = documentSearchService;
        }

        [HttpGet("{query}")]
        public async Task<ActionResult<IEnumerable<DocumentSearchDTO>>> FulltextSearch(string query)
        {
            log.Info($"Performing fulltext search on Elastic with query {query}");
            try
            {
                List<DocumentSearchResult> results = await _documentSearchService.SearchDocumentsAsync(query);
                return Ok(results);
            }
            catch (BusinessLogicException ex)
            {
                log.Error("Business logic error during full-text search.", ex);
                throw new ServiceException("Error in Elasticsearch search service.", ex);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error during full-text search.", ex);
                return StatusCode(500, "Internal server error while searching.");
            }
        }

    }
}