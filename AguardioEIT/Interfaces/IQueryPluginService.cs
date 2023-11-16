using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface IQueryPluginService
{
    Task<QueryResponse<TU>> GetStoredData<T, TU>(Query query, int queryId, SensorType sensorType)
        where T : SensorData where TU : SensorData;
}
