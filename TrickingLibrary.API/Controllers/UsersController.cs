using Microsoft.AspNetCore.Mvc;

namespace TrickingLibrary.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        [HttpGet("me")]
        public IActionResult GetMe() => Ok();
        
        [HttpGet("{id}")]
        public IActionResult GetUser(string id) => Ok();
    }
}