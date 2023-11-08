using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface ISqlDatabasePluginService
{
    Task SaveSensorDataAsync<T>(T data, SensorType sensorType) where T : SensorData;
    Task<SensorData?> GetSensorDataByIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData;
    Task<IEnumerable<SensorData>> GetSensorDataBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData;
}