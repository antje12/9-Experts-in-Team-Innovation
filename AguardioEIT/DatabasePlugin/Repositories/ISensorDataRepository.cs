using Common.Enum;
using Common.Models;

namespace DatabasePlugin.Repositories;

public interface ISensorDataRepository
{
    Task<long> AddDataAsync(IEnumerable<SensorData> data, SensorType sensorType);
    Task<QueryResponse<T>> GetByDataIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData;
    Task<QueryResponse<T>> GetBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData;
}
