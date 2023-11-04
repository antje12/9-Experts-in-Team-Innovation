using System.Data.Odbc;
using Common.Models;
using Interfaces;


namespace HDFS_Plugin;

public class HDFS_Service : IHDFS_Service
{
    private readonly Lazy<OdbcConnection> _odbcConnection;
    private OdbcConnection OdbcConnection => _odbcConnection.Value;


    public HDFS_Service()
    {
        _odbcConnection = new Lazy<OdbcConnection>(() =>
        {
            string connectionString = "DSN=hivedsn";
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
        }
        finally
        {
            await OdbcConnection.CloseAsync();
        }
    }
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
        await ExecuteQueryAsync(hiveQuery);
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
        await ExecuteQueryAsync(hiveQuery);
    }

    public async Task InsertLeakSensorDataAsync(LeakSensorDataSimple data)
    {
        var insertQuery = $@"INSERT INTO TABLE leak_sensor_data
                             VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.DLifeTimeUseCount},
                                     {data.LeakLevelId}, {data.SensorId}, {data.DTemperatureOut}, {data.DTemperatureIn});";
        await ExecuteQueryAsync(insertQuery);
    }

    public async Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data)
    {
        var insertQuery = $@"INSERT INTO TABLE shower_sensor_data
                             VALUES ({data.DataRawId}, '{data.DCreated}', '{data.DReported}', {data.SensorId},
                                     '{data.DShowerState}', {data.DTemperature}, {data.DHumidity}, {data.DBattery});";
        await ExecuteQueryAsync(insertQuery);
    }

    public async Task<List<LeakSensorDataSimple>> LoadLeakSensorData()
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
                        DataRawId = reader.GetInt32(reader.GetOrdinal("DataRaw_id")),
                        DCreated = reader.GetString(reader.GetOrdinal("DCreated")),
                        DReported = reader.GetString(reader.GetOrdinal("DReported")),
                        DLifeTimeUseCount = reader.GetInt32(reader.GetOrdinal("DLifeTimeUseCount")),
                        LeakLevelId = reader.GetInt32(reader.GetOrdinal("LeakLevel_id")),
                        SensorId = reader.GetInt32(reader.GetOrdinal("Sensor_id")),
                        DTemperatureOut = reader.GetFloat(reader.GetOrdinal("DTemperatureOut")),
                        DTemperatureIn = reader.GetFloat(reader.GetOrdinal("DTemperatureIn"))
                    };
                    leakSensorDataList.Add(data);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            await OdbcConnection.CloseAsync();
        }
        return leakSensorDataList;
    }
    public async Task<List<ShowerSensorDataSimple>> LoadShowerSensorData()
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
                        // Assuming the reader can map the database fields directly to the properties of ShowerSensorDataSimple
                        DataRawId = reader.IsDBNull(reader.GetOrdinal("DataRawId")) ? 0 : reader.GetInt32(reader.GetOrdinal("DataRawId")),
                        DCreated = reader.IsDBNull(reader.GetOrdinal("DCreated")) ? string.Empty : reader.GetString(reader.GetOrdinal("DCreated")),
                        DReported = reader.IsDBNull(reader.GetOrdinal("DReported")) ? string.Empty : reader.GetString(reader.GetOrdinal("DReported")),
                        SensorId = reader.IsDBNull(reader.GetOrdinal("SensorId")) ? 0 : reader.GetInt32(reader.GetOrdinal("SensorId")),
                        DShowerState = reader.IsDBNull(reader.GetOrdinal("DShowerState")) ? string.Empty : reader.GetString(reader.GetOrdinal("DShowerState")),
                        DTemperature = reader.IsDBNull(reader.GetOrdinal("DTemperature")) ? 0f : reader.GetFloat(reader.GetOrdinal("DTemperature")),
                        DHumidity = reader.IsDBNull(reader.GetOrdinal("DHumidity")) ? 0 : reader.GetInt32(reader.GetOrdinal("DHumidity")),
                        DBattery = reader.IsDBNull(reader.GetOrdinal("DBattery")) ? 0 : reader.GetInt32(reader.GetOrdinal("DBattery"))
                    };
                    showerSensorDataList.Add(data);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error loading shower sensor data: {e.Message}");
        }
        finally
        {
            await OdbcConnection.CloseAsync();
        }
        return showerSensorDataList;
    }
}