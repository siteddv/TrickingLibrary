using Microsoft.AspNetCore.Mvc;

namespace TrickingLibrary.API.Controllers
{
    [ApiController]
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public string Index()
        {
            return "Hello world";
        }
    }
}