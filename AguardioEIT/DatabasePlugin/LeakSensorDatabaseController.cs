using Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatabasePlugin;

[ApiController]
[Route("[controller]")]
public class LeakSensorDatabaseController : ControllerBase
{
    private readonly LeakSensorSqlDatabasePluginService _leakSensorSqlDatabasePluginService;
    private readonly LeakSensorMongoDatabasePluginService _leakSensorMongoDatabasePluginService;

    public LeakSensorDatabaseController(
        LeakSensorSqlDatabasePluginService leakSensorSqlDatabasePluginService, 
        LeakSensorMongoDatabasePluginService leakSensorMongoDatabasePluginService
    )
    {
        _leakSensorSqlDatabasePluginService = leakSensorSqlDatabasePluginService;
        _leakSensorMongoDatabasePluginService = leakSensorMongoDatabasePluginService;
    }
    
    [HttpPost("sql/add")]
    public async Task<ActionResult> SqlAddSensorData(LeakSensorData data)
    {
        try
        {
            await _leakSensorSqlDatabasePluginService.SaveSensorDataAsync(data);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("sql/getByDataId")]
    public async Task<ActionResult> SqlGetByDataId(int dataId)
    {
        try
        {
            LeakSensorData data = 
                await _leakSensorSqlDatabasePluginService.GetSensorDataByIdAsync(dataId);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("sql/getBySensorId")]
    public async Task<ActionResult> SqlGetBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<LeakSensorData> data = 
                await _leakSensorSqlDatabasePluginService.GetSensorDataBySensorIdAsync(sensorId);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("mongodb/add")]
    public async Task<ActionResult> MongoDbAddSensorData(LeakSensorData data)
    {
        try
        {
            await _leakSensorMongoDatabasePluginService.SaveSensorDataAsync(data);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("mongodb/getByDataId")]
    public async Task<ActionResult> MongoDbGetByDataId(int dataId)
    {
        try
        {
            LeakSensorData data = 
                await _leakSensorMongoDatabasePluginService.GetSensorDataByIdAsync(dataId);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("mongodb/getBySensorId")]
    public async Task<ActionResult> MongoDbGetBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<LeakSensorData> data = 
                await _leakSensorMongoDatabasePluginService.GetSensorDataBySensorIdAsync(sensorId);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}