using Common.Models;
using DatabasePlugin.Context;
using DatabasePlugin.Repositories;

namespace DatabasePlugin;

public class LeakSensorSqlDatabasePluginService
{
    private readonly LeakSensorDataRepository _repo;
    
    public LeakSensorSqlDatabasePluginService(SqlDbContext sqlDbContext)
    {
        _repo = new LeakSensorDataRepository(sqlDbContext);
    }

    public async Task SaveSensorDataAsync(LeakSensorData data)
    {
        await _repo.AddAsync(data);
    }

    public async Task<LeakSensorData> GetSensorDataByIdAsync(int dataId)
    {
        return await _repo.GetByDataIdAsync(dataId);
    }

    public async Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId)
    {
        return await _repo.GetBySensorIdAsync(sensorId);
    }
}
