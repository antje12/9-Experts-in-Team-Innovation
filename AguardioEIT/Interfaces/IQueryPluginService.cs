using Common.Enum;
using Common.Models;

namespace Interfaces;

public interface IQueryPluginService
{
    Task<QueryResponse> GetStoredData<T>(Query query, int queryId, SensorType sensorType) where T : SensorData;
}
