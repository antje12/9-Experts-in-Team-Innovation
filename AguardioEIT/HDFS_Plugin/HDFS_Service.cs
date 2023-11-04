using System.Reflection.PortableExecutable;
using Common.Models;
using Interfaces;
using Microsoft.Hadoop.Hive;

namespace HDFS_Plugin;

public class HDFS_Service : IHDFS_Service
{
    // private  HiveConnection? _hiveConnection;
    private readonly Lazy<HiveConnection> _hiveConnection;
    private HiveConnection HiveConnection => _hiveConnection.Value;

    public HDFS_Service()
    {
        _hiveConnection = new Lazy<HiveConnection>(() =>
        {
            Uri hiveServerUri = new Uri("thrift://hive-server:10000");
            string username = ""; // Replace with your username
            string password = ""; // Replace with your password
            return new HiveConnection(hiveServerUri, username, password);
        });
    }
    // private HiveConnection HiveConnection
    // {
    //     Uri hiveServerUri = new Uri("thrift://hive-server:10000");
    //     string username = ""; 
    //     string password = "";
    //
    //     if (_hiveConnection == null) _hiveConnection = new HiveConnection(hiveServerUri, username, password);
    //
    //     return _hiveConnection;
    // }

    public async Task CreateLeakSensorTableAsync()
    {
        var hiveQuery = @"CREATE TABLE IF NOT EXISTS leak_sensor_data (
                            DataRaw_id INT,
                            DCreated STRING,
                            DReported STRING,
                            DLifeTimeUseCount INT,
                            LeakLevel_id INT,
                            Sensor_id INT,
                            DTemperatureOut FLOAT,
                            DTemperatureIn FLOAT
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ','
                          STORED AS TEXTFILE;";
        try
        {
            await HiveConnection.ExecuteHiveQuery(hiveQuery);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task CreateShowerSensorTableAsync()
    {
        var hiveQuery = @"CREATE TABLE IF NOT EXISTS shower_sensor_data (
                            DataRawId INT,
                            DCreated STRING,
                            DReported STRING,
                            SensorId INT,
                            DShowerState STRING,
                            DTemperature FLOAT,
                            DHumidity INT,
                            DBattery INT
                          )
                          ROW FORMAT DELIMITED
                          FIELDS TERMINATED BY ';'
                          STORED AS TEXTFILE;";

        try
        {
            await HiveConnection.ExecuteHiveQuery(hiveQuery);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task InsertLeakSensorDataAsync(LeakSensorDataSimple data)
    {
        var insertQuery = $@"INSERT INTO TABLE leak_sensor_data
                             VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.DLifeTimeUseCount},
                                     {data.LeakLevelId}, {data.SensorId}, {data.DTemperatureOut}, {data.DTemperatureIn});";

        try
        {
            await HiveConnection.ExecuteHiveQuery(insertQuery);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    public async Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data)
    {
        var insertQuery = $@"INSERT INTO TABLE shower_sensor_data
                             VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.SensorId},
                                     '{data.DShowerState}', {data.DTemperature}, {data.DHumidity}, {data.DBattery});";
        try
        {
            await HiveConnection.ExecuteHiveQuery(insertQuery);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<List<LeakSensorDataSimple>> LoadLeakSensorData()
    {
        var selectQuery = "SELECT * FROM leak_sensor_data;";
        var leakSensorDataList = new List<LeakSensorDataSimple>();

        try
        {
            var resultSet = await HiveConnection.ExecuteHiveQuery<LeakSensorDataSimple>(selectQuery);
            leakSensorDataList = resultSet.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return leakSensorDataList;
    }

    public async Task<List<ShowerSensorDataSimple>> LoadShowerSensorData()
    {
        var selectQuery = "SELECT * FROM shower_sensor_data;";
        var showerSensorDataList = new List<ShowerSensorDataSimple>();

        try
        {
            var resultSet = await HiveConnection.ExecuteHiveQuery<ShowerSensorDataSimple>(selectQuery);
            showerSensorDataList = resultSet.ToList();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        return showerSensorDataList;
    }
}