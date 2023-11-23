using Common.Models;

namespace Interfaces;

public interface IMongoDatabasePluginService
{
    Task<long> SaveSensorDataAsync<T>(IEnumerable<T> data) where T : SensorData;
    Task<QueryResponse<T>> GetSensorDataByIdAsync<T>(int dataId) where T : SensorData;
    Task<QueryResponse<T>> GetSensorDataBySensorIdAsync<T>(int sensorId) where T : SensorData;
}
