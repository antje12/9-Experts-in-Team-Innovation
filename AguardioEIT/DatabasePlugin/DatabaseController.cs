using api.Logging;
using Common.Enum;
using Common.Models;
using DatabasePlugin.Models;
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
      long insertTime = await _sqlDatabasePluginService.SaveSensorDataAsync(leakSensorData, SensorType.LeakSensor);
      await ExperimentLogger.LogAsync(insertTime);
      System.Console.WriteLine($"SqlAddLeakSensorData: {insertTime}ms");
      // return Ok(new InsertResponse { InsertTimeMS = insertTime });
      return Ok();
    }
    catch (Exception e)
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
      long insertTime = await _sqlDatabasePluginService.SaveSensorDataAsync(showerSensorData, SensorType.ShowerSensor);
      System.Console.WriteLine($"SqlAddShowerSensorData: {insertTime}ms");
      await ExperimentLogger.LogAsync(insertTime);
      // return Ok(new InsertResponse { InsertTimeMS = insertTime });
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("sql/leak-sensor/getByDataId")]
  public async Task<ActionResult> SqlGetLeakDataByDataId(int dataId)
  {
    try
    {
      QueryResponse<LeakSensorData> queryResponse =
          await _sqlDatabasePluginService.GetSensorDataByIdAsync<LeakSensorData>(dataId, SensorType.LeakSensor);
      
      System.Console.WriteLine($"SqlGetLeakDataByDataId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);

      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("sql/shower-sensor/getByDataId")]
  public async Task<ActionResult> SqlGetShowerDataByDataId(int dataId)
  {
    try
    {
      QueryResponse<ShowerSensorData> queryResponse =
          await _sqlDatabasePluginService.GetSensorDataByIdAsync<ShowerSensorData>(dataId, SensorType.ShowerSensor);

      Console.WriteLine($"SqlGetShowerDataByDataId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("sql/leak-sensor/getBySensorId")]
  public async Task<ActionResult> SqlGetLeakDataBySensorId(int sensorId)
  {
    try
    {
      QueryResponse<LeakSensorData> queryResponse =
          await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<LeakSensorData>(sensorId, SensorType.LeakSensor);

      Console.WriteLine($"SqlGetLeakDataBySensorId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("sql/shower-sensor/getBySensorId")]
  public async Task<ActionResult> SqlGetShowerDataBySensorId(int sensorId)
  {
    try
    {
      QueryResponse<ShowerSensorData> queryResponse =
          await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<ShowerSensorData>(sensorId, SensorType.ShowerSensor);

      Console.WriteLine($"SqlGetShowerDataBySensorId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
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
      long insertTime = await _mongoDatabasePluginService.SaveSensorDataAsync(leakSensorData);

      Console.WriteLine($"MongoDbAddLeakSensorData: {insertTime}ms");
      await ExperimentLogger.LogAsync(insertTime);

      // return Ok(new InsertResponse { InsertTimeMS = insertTime });
      return Ok();
    }
    catch (Exception e)
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
      long insertTime = await _mongoDatabasePluginService.SaveSensorDataAsync(showerSensorData);

      Console.WriteLine($"MongoDbAddShowerSensorData: {insertTime}ms");
      await ExperimentLogger.LogAsync(insertTime);

      // return Ok(new InsertResponse { InsertTimeMS = insertTime });
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("mongodb/leak-sensor/getByDataId")]
  public async Task<ActionResult> MongoDbGetLeakDataByDataId(int dataId)
  {
    try
    {
      QueryResponse<LeakSensorData> queryResponse = await _mongoDatabasePluginService.GetSensorDataByIdAsync<LeakSensorData>(dataId);

      Console.WriteLine($"MongoDbGetLeakDataByDataId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("mongodb/shower-sensor/getByDataId")]
  public async Task<ActionResult> MongoDbGetShowerDataByDataId(int dataId)
  {
    try
    {
      QueryResponse<ShowerSensorData> queryResponse = await _mongoDatabasePluginService.GetSensorDataByIdAsync<ShowerSensorData>(dataId);

      Console.WriteLine($"MongoDbGetShowerDataByDataId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
    }
    catch (Exception e)
    {
      return BadRequest(e.Message);
    }
  }

  [HttpGet("mongodb/leak-sensor/getBySensorId")]
  public async Task<ActionResult> MongoDbGetLeakDataBySensorId(int sensorId)
  {
    try
    {
      QueryResponse<LeakSensorData> queryResponse =
          await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<LeakSensorData>(sensorId);

      Console.WriteLine($"MongoDbGetLeakDataBySensorId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
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
      QueryResponse<ShowerSensorData> queryResponse =
          await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<ShowerSensorData>(sensorId);

      Console.WriteLine($"MongoDbGetShowerDataBySensorId: Elements: {queryResponse.FetchedItems} {queryResponse.QueryTimeMs}ms");
      await ExperimentLogger.LogAsync(queryResponse.QueryTimeMs);
      // return Ok(queryResponse);
      return Ok();
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
