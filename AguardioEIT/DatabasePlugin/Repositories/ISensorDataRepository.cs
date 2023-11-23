using Common.Enum;
using Common.Models;

namespace DatabasePlugin.Repositories;

public interface ISensorDataRepository
{
    Task<long> AddDataAsync(IEnumerable<SensorData> data, SensorType sensorType);
    Task<QueryResponse<SensorData>> GetByDataIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData;
    Task<QueryResponse<SensorData>> GetBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData;
}
