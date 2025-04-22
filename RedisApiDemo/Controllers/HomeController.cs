using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace RedisApiDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CacheController : ControllerBase
    {
        private readonly IDatabase _redisDb;

        public CacheController(IConnectionMultiplexer redis)
        {
            _redisDb = redis.GetDatabase();
        }

        [HttpGet("set")]
        public IActionResult Set(string key, string value)
        {
            _redisDb.StringSet(key, value);
            return Ok("Value set");
        }

        [HttpGet("get")]
        public IActionResult Get(string key)
        {
            var value = _redisDb.StringGet(key);
            return Ok(value.HasValue ? value.ToString() : "Key not found");
        }
    }
}
