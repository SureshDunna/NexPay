using Microsoft.AspNetCore.Mvc;

namespace NexPayApi.Controllers
{
    [Route("api")]
    public class AccountController : Controller
    {
        [HttpGet("account/get")]
        public IActionResult Get()
        {
            return Ok("This is get on Account Controller");
        }
    }
}