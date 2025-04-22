using Microsoft.AspNetCore.Mvc;

namespace RedisApiDemo.Controllers
{
    [ApiController]
    [Route("health")]
    public class LivenessController : ControllerBase
    {
        [HttpGet("live")]
        public IActionResult Get() => Ok("Alive");
    }
}
