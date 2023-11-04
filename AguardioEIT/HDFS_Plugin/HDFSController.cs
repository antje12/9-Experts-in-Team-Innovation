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

    [HttpGet("LoadLeakSensorData")]
    public async Task<IActionResult> LoadLeakSensorData()
    {
        var data = await _hdfsService.LoadLeakSensorData();
        return Ok(data);
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

    [HttpGet("LoadShowerSensorData")]
    public async Task<IActionResult> LoadShowerSensorData()
    {
        var data = await _hdfsService.LoadShowerSensorData();
        return Ok(data);
    }
    }
}

