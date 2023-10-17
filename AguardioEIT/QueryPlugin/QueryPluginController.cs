using Common.Enum;
using Common.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace QueryPlugin;

[ApiController]
[Route("[controller]")]
public class QueryPluginController : ControllerBase
{
    private readonly IQueryPluginService _queryPluginService;

    public QueryPluginController(IQueryPluginService queryPluginService)
    {
        _queryPluginService = queryPluginService;
    }

    [HttpGet("PerformQuery")]
    public async Task<ActionResult> MongoDbGetBySensorId(Query query, int queryId)
    {
        try
        {
            QueryResponse queryResponse = await _queryPluginService.GetStoredData(query, queryId);
            return Ok(queryResponse);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
