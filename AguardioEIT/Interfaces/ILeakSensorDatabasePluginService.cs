using DatabasePlugin.Enums;
using DatabasePlugin.Models;

namespace Interfaces;

public interface ILeakSensorDatabasePluginService
{
    public Task SaveSensorDataAsync(LeakSensorData data, DatabaseType databaseType);

    public Task<LeakSensorData> GetSensorDataByIdAsync(int dataId, DatabaseType databaseType);

    public Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId, DatabaseType databaseType);
}
