using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface ISqlDatabasePluginService
{
    Task<long> SaveSensorDataAsync<T>(IEnumerable<T> data, SensorType sensorType) where T : SensorData;
    Task<QueryResponse<SensorData>> GetSensorDataByIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData;
    Task<QueryResponse<SensorData>> GetSensorDataBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData;
}