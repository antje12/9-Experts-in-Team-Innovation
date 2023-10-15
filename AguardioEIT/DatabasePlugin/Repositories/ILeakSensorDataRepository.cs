using Common.Models;

namespace DatabasePlugin.Repositories;

public interface ILeakSensorDataRepository
{
    Task AddAsync(LeakSensorData data);
    Task<LeakSensorData> GetByDataIdAsync(int dataId);
    Task<IEnumerable<LeakSensorData>> GetBySensorIdAsync(int sensorId);
}
