using Common.Enum;
using Common.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DatabasePlugin;

[ApiController]
[Route("[controller]")]
public class DatabaseController : ControllerBase
{
    private readonly ISqlDatabasePluginService _sqlDatabasePluginService;
    private readonly IMongoDatabasePluginService _mongoDatabasePluginService;

    public DatabaseController(
        ISqlDatabasePluginService sqlDatabasePluginService, 
        IMongoDatabasePluginService mongoDatabasePluginService
    )
    {
        _sqlDatabasePluginService = sqlDatabasePluginService;
        _mongoDatabasePluginService = mongoDatabasePluginService;
    }
    
    [HttpPost("sql/leak-sensor/add")]
    public async Task<ActionResult> SqlAddLeakSensorData(LeakSensorData data)
    {
        try
        {
            await _sqlDatabasePluginService.SaveSensorDataAsync(data, SensorType.LeakSensor);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("sql/shower-sensor/add")]
    public async Task<ActionResult> SqlAddShowerSensorData(ShowerSensorData data)
    {
        try
        {
            await _sqlDatabasePluginService.SaveSensorDataAsync(data, SensorType.ShowerSensor);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("sql/leak-sensor/getByDataId")]
    public async Task<ActionResult> SqlGetLeakDataByDataId(int dataId)
    {
        try
        {
            SensorData? data = 
                await _sqlDatabasePluginService.GetSensorDataByIdAsync<LeakSensorData>(dataId, SensorType.LeakSensor);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("sql/shower-sensor/getByDataId")]
    public async Task<ActionResult> SqlGetShowerDataByDataId(int dataId)
    {
        try
        {
            SensorData? data = 
                await _sqlDatabasePluginService.GetSensorDataByIdAsync<ShowerSensorData>(dataId, SensorType.ShowerSensor);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("sql/leak-sensor/getBySensorId")]
    public async Task<ActionResult> SqlGetLeakDataBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<SensorData> data = 
                await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<LeakSensorData>(sensorId, SensorType.LeakSensor);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("sql/shower-sensor/getBySensorId")]
    public async Task<ActionResult> SqlGetShowerDataBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<SensorData> data = 
                await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<ShowerSensorData>(sensorId, SensorType.ShowerSensor);
            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("mongodb/leak-sensor/add")]
    public async Task<ActionResult> MongoDbAddLeakSensorData(LeakSensorData data)
    {
        try
        {
            await _mongoDatabasePluginService.SaveSensorDataAsync(data);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("mongodb/shower-sensor/add")]
    public async Task<ActionResult> MongoDbAddShowerSensorData(ShowerSensorData data)
    {
        try
        {
            await _mongoDatabasePluginService.SaveSensorDataAsync(data);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("mongodb/leak-sensor/getByDataId")]
    public async Task<ActionResult> MongoDbGetLeakDataByDataId(int dataId)
    {
        try
        {
            LeakSensorData? data = await _mongoDatabasePluginService.GetSensorDataByIdAsync<LeakSensorData>(dataId);

            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("mongodb/shower-sensor/getByDataId")]
    public async Task<ActionResult> MongoDbGetShowerDataByDataId(int dataId)
    {
        try
        {
            ShowerSensorData? data = await _mongoDatabasePluginService.GetSensorDataByIdAsync<ShowerSensorData>(dataId);

            return Ok(data);
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("mongodb/leak-sensor/getBySensorId")]
    public async Task<ActionResult> MongoDbGetLeakDataBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<LeakSensorData> data = 
                await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<LeakSensorData>(sensorId);

            return Ok(data);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpGet("mongodb/shower-sensor/getBySensorId")]
    public async Task<ActionResult> MongoDbGetShowerDataBySensorId(int sensorId)
    {
        try
        {
            IEnumerable<ShowerSensorData> data = 
                await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<ShowerSensorData>(sensorId);

            return Ok(data);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}
