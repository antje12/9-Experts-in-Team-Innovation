using Common.Models;

namespace Interfaces;

public interface ILeakSensorMongoDatabasePluginService
{
    Task SaveSensorDataAsync(LeakSensorData data); 
    Task<LeakSensorData> GetSensorDataByIdAsync(int dataId);
    Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId);
}
