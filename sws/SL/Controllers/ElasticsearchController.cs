using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sws.SL.DTOs;
using sws.BLL;

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
            List<DocumentSearchDTO> results = await _documentSearchService.SearchDocumentsAsync(query);
            return Ok(results);
        }
    }
}