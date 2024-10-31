using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;

namespace sws.SL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]


    public class HelloWorldController : ControllerBase
    {
        /// <summary>
        /// Simple Hello World endpoint
        /// </summary>

        /// <remarks>
        /// Simply returns a hardcodeds greeting message
        /// </remarks>

        [HttpGet]
        public string Get()
        {
            return "Hello This World";
        }
    }
}
