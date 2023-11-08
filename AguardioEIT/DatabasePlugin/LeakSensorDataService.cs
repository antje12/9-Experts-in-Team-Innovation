using Common.Models;
using DatabasePlugin.Context;
using DatabasePlugin.Interfaces;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DatabasePlugin;

public class LeakSensorDataService : ISensorDataService<LeakSensorData>
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly IRedisPluginService _redisPluginService;

    public LeakSensorDataService(MongoDbContext mongoDbContext, IRedisPluginService redisPluginService)
    {
        _mongoDbContext = mongoDbContext;
        _redisPluginService = redisPluginService;
    }

    public async Task SaveSensorDataAsync(LeakSensorData data)
    {
        await _mongoDbContext.LeakSensorData.InsertOneAsync(data);
        await _redisPluginService.SetAsync($"MongoDb:LeakSensor:DataId={data.DataRawId}", JsonConvert.SerializeObject(data));

        IEnumerable<LeakSensorData> sensorDataCollection = await GetSensorDataBySensorIdAsync(data.SensorId);
        await _redisPluginService.SetAsync($"MongoDb:LeakSensor:SensorId={data.SensorId}", JsonConvert.SerializeObject(sensorDataCollection));
    }

    public async Task<LeakSensorData> GetSensorDataByIdAsync(int dataId)
    {
        IQueryable<LeakSensorData> query = _mongoDbContext.LeakSensorData.AsQueryable().Where(x => x.DataRawId == dataId);
        LeakSensorData? data = await query.FirstOrDefaultAsync();
    
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return data;
    }

    public async Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId)
    {
        IQueryable<LeakSensorData> query = _mongoDbContext.LeakSensorData.AsQueryable().Where(x => x.SensorId == sensorId);
        List<LeakSensorData> data = await query.ToListAsync();
        return data;
    }
}
