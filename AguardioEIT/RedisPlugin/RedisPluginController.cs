using Interfaces;
using Microsoft.AspNetCore.Mvc;
using RedisPlugin.DTO;

namespace RedisPlugin;

[ApiController]
[Route("[controller]")]
public class RedisPluginController : ControllerBase
{
    private readonly IRedisPluginService _redisService;

    public RedisPluginController(IRedisPluginService redisService)
    {
        _redisService = redisService;
    }
    
    [HttpPost]
    public async Task<ActionResult> Set(SetCacheDto cache)
    {
        try
        {
            await _redisService.SetAsync(cache.Key, cache.Value, cache.ExpirationSeconds);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet]
    public async Task<ActionResult<GetCacheResultDto>> Get([FromQuery] GetCacheRequestDto request)
    {
        try
        {
            string value = await _redisService.GetAsync(request.Key);
            return Ok(new GetCacheResultDto { Value = value });
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
