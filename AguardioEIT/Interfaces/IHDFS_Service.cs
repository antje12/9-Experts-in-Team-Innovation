using Common.Models;

namespace Interfaces;

public interface IHDFS_Service
{
  Task CreateHiveTables();
  Task InsertLeakSensorDataAsync(LeakSensorDataSimple data);
  Task InsertShowerSensorDataAsync(ShowerSensorDataSimple data);
  Task InsertLeakSensorDataAsync(List<LeakSensorDataSimple> data);
  Task InsertShowerSensorDataAsync(List<ShowerSensorDataSimple> data);
  Task<List<LeakSensorDataSimple>> LoadAllLeakSensorDataAsync();
  Task<List<ShowerSensorDataSimple>> LoadAllShowerSensorDataAsync();
}