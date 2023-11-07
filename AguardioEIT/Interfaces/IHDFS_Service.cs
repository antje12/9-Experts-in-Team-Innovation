using Common.Models;

namespace Interfaces;

public interface IHDFS_Service
{
  Task CreateLeakSensorTableAsync();
  Task CreateShowerSensorTableAsync();
  Task InsertLeakSensorDataAsync(LeakSensorDataSimple data);
  Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data);
  Task<List<LeakSensorDataSimple>> LoadLeakSensorDataAsync();
  Task<List<ShowerSensorDataSimple>> LoadShowerSensorDataAsync();
}