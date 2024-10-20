using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace sws.SL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HelloWorldController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "Hello World";
        }
    }
}
