using Common.Models;
using Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HDFS_Plugin
{
  [ApiController]
  [Route("[controller]")]
  public class HDFSController : ControllerBase
  {
    private readonly IHDFS_Service _hdfsService;

    public HDFSController(IHDFS_Service hdfsService)
    {
      _hdfsService = hdfsService;
    }

    [HttpGet("CreateHiveTables")]
    public async Task<IActionResult> CreateHiveTables()
    {
      await _hdfsService.CreateHiveTables();
      return Ok();
    }

    [HttpPost("LeakSensorData/Add")]
    public async Task<IActionResult> InsertLeakSensorData()
    {
      var dummyLeakData = new LeakSensorDataSimple
      {
        DataRawId = 1,
        DCreated = "2023-11-04",
        DReported = "2023-11-04",
        DLifeTimeUseCount = 100,
        LeakLevelId = 2,
        SensorId = 1,
        DTemperatureOut = 25.5f,
        DTemperatureIn = 22.3f
      };

      await _hdfsService.InsertLeakSensorDataAsync(dummyLeakData);

      return Ok("Leak sensor data inserted successfully.");
    }

    [HttpPost("LeakSensorData/Add/{count:int}")]
    public async Task<IActionResult> InsertLeakSensorDataBulk(int count)
    {
      var data = new List<LeakSensorDataSimple>();
      Random rnd = new Random();
      var sensorId = 1;

      for (int i = 0; i < count; i++)
      {
        if ((i != 0) && (i % 10 == 0)) sensorId++;
        var randomInt = rnd.Next(1, 100);
        var dummyLeakData = new LeakSensorDataSimple
        {
          DataRawId = i + 1,
          DCreated = "2023-11-04",
          DReported = "2023-11-04",
          DLifeTimeUseCount = randomInt,
          LeakLevelId = 2,
          SensorId = sensorId,
          DTemperatureOut = 25.5f,
          DTemperatureIn = 22.3f
        };
        data.Add(dummyLeakData);
      }

      await _hdfsService.InsertLeakSensorDataAsync(data);

      return Ok($"{count} rows of Leak sensor data inserted successfully.");
    }

    [HttpGet("LeakSensorData/GetAll")]
    public async Task<IActionResult> LoadLeakSensorData()
    {
      var data = await _hdfsService.LoadAllLeakSensorDataAsync();
      return Ok(data);
    }

    [HttpGet("LeakSensorData/GetBySensorId/{sensorId}")]
    public async Task<IActionResult> GetLeakSensorDataBySensorId(int sensorId)
    {
      try
      {
        var data = await _hdfsService.LoadLeakSensorDataBySensorIdAsync(sensorId);
        if (data != null && data.Count > 0)
        {
          return Ok(data);
        }
        else
        {
          return NotFound($"No leak sensor data found for sensor ID {sensorId}.");
        }
      }
      catch (Exception e)
      {
        return StatusCode(500, $"Internal server error: {e.Message}");
      }
    }

    [HttpGet("LeakSensorData/GetByDataId/{dataRawId}")]
    public async Task<IActionResult> GetLeakSensorDataByDataId(int dataRawId)
    {
      try
      {
        var data = await _hdfsService.LoadLeakSensorDataByDataRawIdAsync(dataRawId);
        if (data != null)
        {
          return Ok(data);
        }
        else
        {
          return NotFound($"No leak sensor data found for data ID {dataRawId}.");
        }
      }
      catch (Exception e)
      {
        return StatusCode(500, $"Internal server error: {e.Message}");
      }
    }

    [HttpPost("ShowerSensorData/Add")]
    public async Task<IActionResult> InsertShowerSensorData()
    {
      var dummyShowerData = new ShowerSensorDataSimple
      {
        DataRawId = 1,
        DCreated = "2023-11-04",
        DReported = "2023-11-04",
        SensorId = 1,
        DShowerState = "On",
        DTemperature = 35.5f,
        DHumidity = 80,
        DBattery = 90
      };

      await _hdfsService.InsertShowerSensorDataAsync(dummyShowerData);

      return Ok("Shower sensor data inserted successfully.");
    }

    [HttpPost("ShowerSensorData/Add/{count:int}")]
    public async Task<IActionResult> InsertShowerSensorDataBulk(int count)
    {
      var data = new List<ShowerSensorDataSimple>();
      var sensorId = 1;
      for (int i = 0; i < count; i++)
      {
        if ((i != 0) && (i % 10 == 0)) sensorId++;
        var dummyLeakData = new ShowerSensorDataSimple
        {
          DataRawId = i + 1,
          DCreated = "2023-11-04",
          DReported = "2023-11-04",
          SensorId = sensorId,
          DShowerState = "On",
          DTemperature = 35.5f,
          DHumidity = 80,
          DBattery = 90
        };
        data.Add(dummyLeakData);
      }

      await _hdfsService.InsertShowerSensorDataAsync(data);

      return Ok($"{count} rows of Shower sensor data inserted successfully.");
    }

    [HttpGet("ShowerSensorData/GetAll")]
    public async Task<IActionResult> LoadShowerSensorData()
    {
      var data = await _hdfsService.LoadAllShowerSensorDataAsync();
      return Ok(data);
    }

    [HttpGet("ShowerSensorData/GetBySensorId/{sensorId}")]
    public async Task<IActionResult> GetShowerSensorDataBySensorId(int sensorId)
    {
      try
      {
        var data = await _hdfsService.LoadShowerSensorDataBySensorIdAsync(sensorId);
        if (data != null && data.Count > 0)
        {
          return Ok(data);
        }
        else
        {
          return NotFound($"No shower sensor data found for sensor ID {sensorId}.");
        }
      }
      catch (Exception e)
      {
        // Log the exception as needed
        return StatusCode(500, $"Internal server error: {e.Message}");
      }
    }

    [HttpGet("ShowerSensorData/GetByDataId/{dataRawId}")]
    public async Task<IActionResult> GetShowerSensorDataByDataId(int dataRawId)
    {
      try
      {
        var data = await _hdfsService.LoadShowerSensorDataByDataRawIdAsync(dataRawId);
        if (data != null)
        {
          return Ok(data);
        }
        else
        {
          return NotFound($"No shower sensor data found for data ID {dataRawId}.");
        }
      }
      catch (Exception e)
      {
        // Log the exception as needed
        return StatusCode(500, $"Internal server error: {e.Message}");
      }
    }
  }
}

