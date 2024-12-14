using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sws.SL.DTOs;

namespace sws.SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ElasticsearchController : Controller
    {
        // GET: ElasticsearchController
        // TODO
        [HttpGet]
        public IEnumerable<String> FulltextSearch()
        {
            return new List<String> { "Mock", "Return" };
        }
    }
}