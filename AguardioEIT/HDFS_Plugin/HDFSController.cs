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

    [HttpPost("InsertLeakSensorData")]
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
    
    [HttpPost("LeakSensorData/Insert/Bulk/{int}")]
    public async Task<IActionResult> InsertLeakSensorDataBulk(int count)
    {
      var data = new List<LeakSensorDataSimple>();
      
      for (int i = 0; i < count; i++)
      {
        Random rnd = new Random();
        var randomInt = rnd.Next(1, 100);
        var dummyLeakData = new LeakSensorDataSimple
        {
          DataRawId = i+1,
          DCreated = "2023-11-04",
          DReported = "2023-11-04",
          DLifeTimeUseCount = randomInt,
          LeakLevelId = 2,
          SensorId = 1,
          DTemperatureOut = 25.5f,
          DTemperatureIn = 22.3f
        };
        data.Add(dummyLeakData);
      }

      await _hdfsService.InsertLeakSensorDataAsync(data);

      return Ok($"{count} rows of Leak sensor data inserted successfully.");
    }

    [HttpGet("LoadLeakSensorData")]
    public async Task<IActionResult> LoadLeakSensorData()
    {
      var data = await _hdfsService.LoadAllLeakSensorDataAsync();
      return Ok(data);
    }

    [HttpGet("CreateHiveTables")]
    public async Task<IActionResult> CreateHiveTables()
    {
      await _hdfsService.CreateHiveTables();

      return Ok();
    }

    [HttpPost("InsertShowerSensorData")]
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
    
    [HttpPost("ShowerSensorData/Insert/Bulk/{int}")]
    public async Task<IActionResult> InsertShowerSensorDataBulk(int count)
    {
      var data = new List<ShowerSensorDataSimple>();
      
      for (int i = 0; i < count; i++)
      {
        var dummyLeakData = new ShowerSensorDataSimple
        {
          DataRawId = i+1,
          DCreated = "2023-11-04",
          DReported = "2023-11-04",
          SensorId = i+1,
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

    [HttpGet("LoadShowerSensorData")]
    public async Task<IActionResult> LoadShowerSensorData()
    {
      var data = await _hdfsService.LoadAllShowerSensorDataAsync();
      return Ok(data);
    }
  }
}

