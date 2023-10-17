using Common.Models;
using DatabasePlugin.Context;
using DatabasePlugin.Repositories;
using Interfaces;
using Newtonsoft.Json;

namespace DatabasePlugin;

public sealed class LeakSensorSqlDatabasePluginService : ILeakSensorSqlDatabasePluginService
{
    private readonly IRedisPluginService _redisPluginService;
    private readonly LeakSensorDataRepository _repo;
    
    public LeakSensorSqlDatabasePluginService(SqlDbContext sqlDbContext, IRedisPluginService redisPluginService)
    {
        _redisPluginService = redisPluginService;
        _repo = new LeakSensorDataRepository(sqlDbContext);
    }

    public async Task SaveSensorDataAsync(LeakSensorData data)
    {
        await _repo.AddAsync(data);
        await _redisPluginService.SetAsync($"Sql:DataId={data.DataRawId}", JsonConvert.SerializeObject(data));

        IEnumerable<LeakSensorData> sensorDataCollection = await GetSensorDataBySensorIdAsync(data.SensorId);
        await _redisPluginService.SetAsync($"Sql:SensorId={data.SensorId}", JsonConvert.SerializeObject(sensorDataCollection));
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
