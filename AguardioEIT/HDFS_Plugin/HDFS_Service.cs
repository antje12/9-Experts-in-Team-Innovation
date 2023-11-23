using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Interfaces;

namespace HDFS_Plugin
{
  public class HDFS_Service : IHDFS_Service
  {
    private readonly Lazy<OdbcConnection> _odbcConnection;
    private OdbcConnection OdbcConnection => _odbcConnection.Value;

    public HDFS_Service()
    {
      _odbcConnection = new Lazy<OdbcConnection>(() =>
      {
        string connectionString = "Driver=Hive;Host=hive-server;Port=10000;HiveServerType=2;";
        return new OdbcConnection(connectionString);
      });
    }

    private async Task ExecuteQueryAsync(string queryString)
    {
      try
      {
        // await OdbcConnection.OpenAsync(); // Remember to remove
        using (var command = new OdbcCommand(queryString, OdbcConnection))
        {
          await command.ExecuteNonQueryAsync();
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
        throw;
      }
      // finally
      // {
      //   await OdbcConnection.CloseAsync(); // Remember to remove
      // }
    }

    public async Task CreateHiveTables()
    {
      await OdbcConnection.OpenAsync();
      var hiveLeakQuery = @"CREATE TABLE IF NOT EXISTS leak_sensor_data (
                            dataraw_id INT,
                            dcreated STRING,
                            dreported STRING,
                            dlifetimeusecount STRING,
                            leaklevel_id INT,
                            sensor_id INT,
                            dtemperatureout STRING,
                            dtemperaturein STRING
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ','
                          STORED AS TEXTFILE;";
      await ExecuteQueryAsync(hiveLeakQuery);

      var hiveShowerQuery = @"CREATE TABLE IF NOT EXISTS shower_sensor_data (
                            datarawid INT,
                            dcreated STRING,
                            dreported STRING,
                            sensorid INT,
                            dshowerstate STRING,
                            dtemperature STRING,
                            dhumidity STRING,
                            dbattery STRING
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ';'
                          STORED AS TEXTFILE;";
      await ExecuteQueryAsync(hiveShowerQuery);

      await OdbcConnection.CloseAsync();
    }

    public async Task InsertLeakSensorDataAsync(LeakSensorDataSimple data)
    {
      await OdbcConnection.OpenAsync();
      var insertQuery = $@"INSERT INTO leak_sensor_data
                                 VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', '{data.DLifeTimeUseCount}',
                                         {data.LeakLevelId}, {data.SensorId}, '{data.DTemperatureOut}', '{data.DTemperatureIn}');";
      await ExecuteQueryAsync(insertQuery);
      await OdbcConnection.CloseAsync();
    }

    public async Task InsertLeakSensorDataAsync(List<LeakSensorDataSimple> data)
    {
      await OdbcConnection.OpenAsync();
      var insertCommandText = new StringBuilder("INSERT INTO leak_sensor_data VALUES ");
      var insertValues = new List<string>();
      foreach (var sensorData in data)
      {
        insertValues.Add($"({sensorData.DataRawId}, '{sensorData.DCreated}', '{sensorData.DReported}', '{sensorData.DLifeTimeUseCount}', {sensorData.LeakLevelId}, {sensorData.SensorId}, '{sensorData.DTemperatureOut}', '{sensorData.DTemperatureIn}')");
      }
      insertCommandText.Append(string.Join(",", insertValues));
      insertCommandText.Append(";");

      await ExecuteQueryAsync(insertCommandText.ToString());
      await OdbcConnection.CloseAsync();
    }

    public async Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data)
    {
      await OdbcConnection.OpenAsync();
      var insertQuery = $@"INSERT INTO shower_sensor_data
                                 VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.SensorId},
                                         '{data.DShowerState}', '{data.DTemperature}', '{data.DHumidity}', '{data.DBattery}');";
      await ExecuteQueryAsync(insertQuery);
      await OdbcConnection.CloseAsync();
    }
    public async Task InsertShowerSensorDataAsync(List<ShowerSensorDataSimple> data)
    {
      await OdbcConnection.OpenAsync();
      var insertCommandText = new StringBuilder("INSERT INTO shower_sensor_data VALUES ");

      var insertValues = new List<string>();
      foreach (var sensorData in data)
      {
        insertValues.Add($"({sensorData.DataRawId}, '{sensorData.DCreated}', '{sensorData.DReported}', {sensorData.SensorId}, '{sensorData.DShowerState}', '{sensorData.DTemperature}', '{sensorData.DHumidity}', '{sensorData.DBattery}')");
      }
      insertCommandText.Append(string.Join(",", insertValues));
      insertCommandText.Append(";");

      await ExecuteQueryAsync(insertCommandText.ToString());
      await OdbcConnection.CloseAsync();
    }

    public async Task<List<LeakSensorDataSimple>> LoadAllLeakSensorDataAsync()
    {
      var selectQuery = "SELECT * FROM leak_sensor_data;";
      var leakSensorDataList = new List<LeakSensorDataSimple>();

      try
      {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            var data = new LeakSensorDataSimple
            {
              DataRawId = reader.GetInt32(0),
              DCreated = reader.GetString(1),
              DReported = reader.GetString(2),
              DLifeTimeUseCount = reader.GetString(3),
              LeakLevelId = reader.GetInt32(4),
              SensorId = reader.GetInt32(5),
              DTemperatureOut = reader.GetString(6),
              DTemperatureIn = reader.GetString(7)
            };
            leakSensorDataList.Add(data);
          }
        }
        stopwatch.Stop();
        System.Console.WriteLine($"Joo joo det tog sådan ca: {stopwatch.ElapsedMilliseconds} ms at hente {leakSensorDataList.Count} ting bum bum");
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading leak sensor data: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }
      return leakSensorDataList;
    }

    public async Task<List<LeakSensorDataSimple>> LoadLeakSensorDataBySensorIdAsync(int sensorId)
    {
      var selectQuery = $"SELECT dataraw_id, dcreated, dreported, dlifetimeusecount, leaklevel_id, sensor_id, dtemperatureout, dtemperaturein FROM leak_sensor_data WHERE sensor_id = {sensorId};";

      var leakSensorDataList = new List<LeakSensorDataSimple>();

      try
      {
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        {
          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var data = new LeakSensorDataSimple
              {
                DataRawId = reader.GetInt32(0),
                DCreated = reader.GetString(1),
                DReported = reader.GetString(2),
                DLifeTimeUseCount = reader.GetString(3),
                LeakLevelId = reader.GetInt32(4),
                SensorId = reader.GetInt32(5),
                DTemperatureOut = reader.GetString(6),
                DTemperatureIn = reader.GetString(7)
              };
              leakSensorDataList.Add(data);
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading leak sensor data for sensor ID {sensorId}: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }
      return leakSensorDataList;
    }

    public async Task<LeakSensorDataSimple> LoadLeakSensorDataByDataRawIdAsync(int dataRawId)
    {
      var selectQuery = $"SELECT dataraw_id, dcreated, dreported, dlifetimeusecount, leaklevel_id, sensor_id, dtemperatureout, dtemperaturein FROM leak_sensor_data WHERE dataraw_id = {dataRawId};";
      LeakSensorDataSimple data = null;

      try
      {
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        {
          using (var reader = await command.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              data = new LeakSensorDataSimple
              {
                DataRawId = reader.GetInt32(0),
                DCreated = reader.GetString(1),
                DReported = reader.GetString(2),
                DLifeTimeUseCount = reader.GetString(3),
                LeakLevelId = reader.GetInt32(4),
                SensorId = reader.GetInt32(5),
                DTemperatureOut = reader.GetString(6),
                DTemperatureIn = reader.GetString(7)
              };
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading leak sensor data for data ID {dataRawId}: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }

      return data;
    }

    public async Task<List<ShowerSensorDataSimple>> LoadAllShowerSensorDataAsync()
    {
      var showerSensorDataList = new List<ShowerSensorDataSimple>();
      string selectQuery = "SELECT * FROM shower_sensor_data;";

      try
      {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            var data = new ShowerSensorDataSimple
            {
              DataRawId = reader.GetInt32(0),
              DCreated = reader.GetString(1),
              DReported = reader.GetString(2),
              SensorId = reader.GetInt32(3),
              DShowerState = reader.GetString(4),
              DTemperature = reader.GetString(5),
              DHumidity = reader.GetString(6),
              DBattery = reader.GetString(7)
            };
            showerSensorDataList.Add(data);
          }
        }
        stopwatch.Stop();
        System.Console.WriteLine($"Joo joo det tog sådan ca: {stopwatch.ElapsedMilliseconds} ms at hente {showerSensorDataList.Count} ting bum bum");
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading shower sensor data: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }
      return showerSensorDataList;
    }

    public async Task<List<ShowerSensorDataSimple>> LoadShowerSensorDataBySensorIdAsync(int sensorId)
    {
      var showerSensorDataList = new List<ShowerSensorDataSimple>();
      string selectQuery = $"SELECT datarawid, dcreated, dreported, sensorid, dshowerstate, dtemperature, dhumidity, dbattery FROM shower_sensor_data WHERE sensorid = {sensorId};";

      try
      {
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        {
          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              var data = new ShowerSensorDataSimple
              {
                DataRawId = reader.GetInt32(0),
                DCreated = reader.GetString(1),
                DReported = reader.GetString(2),
                SensorId = reader.GetInt32(3),
                DShowerState = reader.GetString(4),
                DTemperature = reader.GetString(5),
                DHumidity = reader.GetString(6),
                DBattery = reader.GetString(7)
              };
              showerSensorDataList.Add(data);
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading shower sensor data for sensor ID {sensorId}: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }
      return showerSensorDataList;
    }

    public async Task<ShowerSensorDataSimple> LoadShowerSensorDataByDataRawIdAsync(int dataRawId)
    {
      ShowerSensorDataSimple data = null;
      string selectQuery = $"SELECT datarawid, dcreated, dreported, sensorid, dshowerstate, dtemperature, dhumidity, dbattery FROM shower_sensor_data WHERE datarawid = {dataRawId};";

      try
      {
        await OdbcConnection.OpenAsync();
        using (var command = new OdbcCommand(selectQuery, OdbcConnection))
        {
          using (var reader = await command.ExecuteReaderAsync())
          {
            if (await reader.ReadAsync())
            {
              data = new ShowerSensorDataSimple
              {
                DataRawId = reader.GetInt32(0),
                DCreated = reader.GetString(1),
                DReported = reader.GetString(2),
                SensorId = reader.GetInt32(3),
                DShowerState = reader.GetString(4),
                DTemperature = reader.GetString(5),
                DHumidity = reader.GetString(6),
                DBattery = reader.GetString(7)
              };
            }
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine($"Error loading shower sensor data for data ID {dataRawId}: {e.Message}");
        throw;
      }
      finally
      {
        await OdbcConnection.CloseAsync();
      }

      return data;
    }

  }
}
