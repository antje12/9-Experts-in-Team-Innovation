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
    public async Task<ActionResult> SqlAddLeakSensorData(LeakSensorData data, int repeat = 1)
    {
        try
        {
            IEnumerable<LeakSensorData> leakSensorData = GenerateLeakSensorData(repeat, data);
            await _sqlDatabasePluginService.SaveSensorDataAsync(leakSensorData, SensorType.LeakSensor);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("sql/shower-sensor/add")]
    public async Task<ActionResult> SqlAddShowerSensorData(ShowerSensorData data, int repeat = 1)
    {
        try
        {
            IEnumerable<ShowerSensorData> showerSensorData = GenerateShowerSensorData(repeat, data);
            await _sqlDatabasePluginService.SaveSensorDataAsync(showerSensorData, SensorType.ShowerSensor);
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
    public async Task<ActionResult> MongoDbAddLeakSensorData(LeakSensorData data, int repeat = 1)
    {
        try
        {
            IEnumerable<LeakSensorData> leakSensorData = GenerateLeakSensorData(repeat, data);
            await _mongoDatabasePluginService.SaveSensorDataAsync(leakSensorData);
            return Ok();
        } catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    [HttpPost("mongodb/shower-sensor/add")]
    public async Task<ActionResult> MongoDbAddShowerSensorData(ShowerSensorData data, int repeat = 1)
    {
        try
        {
            IEnumerable<ShowerSensorData> showerSensorData = GenerateShowerSensorData(repeat, data);
            await _mongoDatabasePluginService.SaveSensorDataAsync(showerSensorData);
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

    private static IEnumerable<LeakSensorData> GenerateLeakSensorData(int count, LeakSensorData template)
    {
        List<LeakSensorData> leakSensorDataList = new();
        
        for (int i = 0; i < count; i++)
        {
            LeakSensorData newData = new()
            {
                DCreated = template.DCreated,
                SensorId = template.SensorId,
                DReported = template.DReported,
                DTemperatureIn = template.DTemperatureIn,
                DTemperatureOut = template.DTemperatureOut,
                LeakLevelId = template.LeakLevelId,
                DLifeTimeUseCount = template.DLifeTimeUseCount
            };
            leakSensorDataList.Add(newData);
        }
        
        return leakSensorDataList;
    }
    
    private static IEnumerable<ShowerSensorData> GenerateShowerSensorData(int count, ShowerSensorData template)
    {
        List<ShowerSensorData> leakSensorDataList = new();
        
        for (int i = 0; i < count; i++)
        {
            ShowerSensorData newData = new()
            {
                DCreated = template.DCreated,
                SensorId = template.SensorId,
                DReported = template.DReported,
                DTemperature = template.DTemperature,
                DBattery = template.DBattery,
                DHumidity = template.DHumidity,
                DShowerState = template.DShowerState
            };
            leakSensorDataList.Add(newData);
        }
        
        return leakSensorDataList;
    }
}
