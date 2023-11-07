using System;
using System.Collections.Generic;
using System.Data.Odbc;
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
        await OdbcConnection.OpenAsync();
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
      finally
      {
        await OdbcConnection.CloseAsync();
      }
    }

    public async Task CreateLeakSensorTableAsync()
    {
      var hiveQuery = @"CREATE TABLE IF NOT EXISTS leak_sensor_data (
                            dataraw_id INT,
                            dcreated STRING,
                            dreported STRING,
                            dlifetimeusecount INT,
                            leaklevel_id INT,
                            sensor_id INT,
                            dtemperatureout FLOAT,
                            dtemperaturein FLOAT
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ','
                          STORED AS TEXTFILE;";
      await ExecuteQueryAsync(hiveQuery);
    }

    public async Task CreateShowerSensorTableAsync()
    {
      var hiveQuery = @"CREATE TABLE IF NOT EXISTS shower_sensor_data (
                            datarawid INT,
                            dcreated STRING,
                            dreported STRING,
                            sensorid INT,
                            dshowerstate STRING,
                            dtemperature FLOAT,
                            dhumidity INT,
                            dbattery INT
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ';'
                          STORED AS TEXTFILE;";
      await ExecuteQueryAsync(hiveQuery);
    }

    public async Task InsertLeakSensorDataAsync(LeakSensorDataSimple data)
    {
      var insertQuery = $@"INSERT INTO leak_sensor_data
                                 VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.DLifeTimeUseCount},
                                         {data.LeakLevelId}, {data.SensorId}, {data.DTemperatureOut}, {data.DTemperatureIn});";
      await ExecuteQueryAsync(insertQuery);
    }

    public async Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data)
    {
      var insertQuery = $@"INSERT INTO shower_sensor_data
                                 VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.SensorId},
                                         '{data.DShowerState}', {data.DTemperature}, {data.DHumidity}, {data.DBattery});";
      await ExecuteQueryAsync(insertQuery);
    }

    public async Task<List<LeakSensorDataSimple>> LoadLeakSensorDataAsync()
    {
      var selectQuery = "SELECT * FROM leak_sensor_data;";
      var leakSensorDataList = new List<LeakSensorDataSimple>();

      try
      {
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
              DLifeTimeUseCount = reader.GetInt32(3),
              LeakLevelId = reader.GetInt32(4),
              SensorId = reader.GetInt32(5),
              DTemperatureOut = reader.GetFloat(6),
              DTemperatureIn = reader.GetFloat(7)
              // DataRawId = reader.GetInt32(reader.GetOrdinal("dataraw_id")),
              // DCreated = reader.GetString(reader.GetOrdinal("dcreated")),
              // DReported = reader.GetString(reader.GetOrdinal("dreported")),
              // DLifeTimeUseCount = reader.GetInt32(reader.GetOrdinal("dlifetimeusecount")),
              // LeakLevelId = reader.GetInt32(reader.GetOrdinal("leaklevel_id")),
              // SensorId = reader.GetInt32(reader.GetOrdinal("sensor_id")),
              // DTemperatureOut = reader.GetFloat(reader.GetOrdinal("dtemperatureout")),
              // DTemperatureIn = reader.GetFloat(reader.GetOrdinal("dtemperaturein"))
            };
            leakSensorDataList.Add(data);
          }
        }
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

    public async Task<List<ShowerSensorDataSimple>> LoadShowerSensorDataAsync()
    {
      var showerSensorDataList = new List<ShowerSensorDataSimple>();
      string selectQuery = "SELECT * FROM shower_sensor_data;";

      try
      {
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
              DTemperature = reader.GetFloat(5),
              DHumidity = reader.GetInt32(6),
              DBattery = reader.GetInt32(7)
            };
            showerSensorDataList.Add(data);
          }
        }
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
  }
}
