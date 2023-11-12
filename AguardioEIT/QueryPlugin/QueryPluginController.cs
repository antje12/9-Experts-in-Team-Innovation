using System.Diagnostics;
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
    public async Task<ActionResult<dynamic>> MongoDbGetBySensorId(Query query, int queryId, SensorType sensorType)
    {
        try
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();

            dynamic queryResponse = sensorType switch
            {
                SensorType.LeakSensor => await GetStoredDataAsync<LeakSensorData>(query, queryId, sensorType),
                SensorType.ShowerSensor => await GetStoredDataAsync<ShowerSensorData>(query, queryId, sensorType),
                _ => throw new ArgumentException("Invalid sensor type.")
            };

            stopwatch.Stop();
            queryResponse.QueryTimeMS = stopwatch.ElapsedMilliseconds;
            queryResponse.FetchedItems = queryResponse.Data.Count;
            return Ok(queryResponse);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    private async Task<QueryResponse<T>> GetStoredDataAsync<T>(Query query, int queryId, SensorType sensorType) where T : SensorData
    {
        return await _queryPluginService.GetStoredData<T, T>(query, queryId, sensorType);
    }
}
