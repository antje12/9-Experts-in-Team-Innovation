using Common.Models;
using DatabasePlugin.Context;
using DatabasePlugin.Interfaces;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace DatabasePlugin;

public class ShowerSensorDataService : ISensorDataService<ShowerSensorData>
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly IRedisPluginService _redisPluginService;

    public ShowerSensorDataService(MongoDbContext mongoDbContext, IRedisPluginService redisPluginService)
    {
        _mongoDbContext = mongoDbContext;
        _redisPluginService = redisPluginService;
    }

    public async Task SaveSensorDataAsync(ShowerSensorData data)
    {
        await _mongoDbContext.ShowerSensorData.InsertOneAsync(data);
        await _redisPluginService.SetAsync($"MongoDb:ShowerSensor:DataId={data.DataRawId}", JsonConvert.SerializeObject(data));

        IEnumerable<ShowerSensorData> sensorDataCollection = await GetSensorDataBySensorIdAsync(data.SensorId);
        await _redisPluginService.SetAsync($"MongoDb:ShowerSensor:SensorId={data.SensorId}", JsonConvert.SerializeObject(sensorDataCollection));
    }

    public async Task<ShowerSensorData> GetSensorDataByIdAsync(int dataId)
    {
        IQueryable<ShowerSensorData> query = _mongoDbContext.ShowerSensorData.AsQueryable().Where(x => x.DataRawId == dataId);
        ShowerSensorData? data = await query.FirstOrDefaultAsync();
    
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return data;
    }

    public async Task<IEnumerable<ShowerSensorData>> GetSensorDataBySensorIdAsync(int sensorId)
    {
        IQueryable<ShowerSensorData> query = _mongoDbContext.ShowerSensorData.AsQueryable().Where(x => x.SensorId == sensorId);
        List<ShowerSensorData> data = await query.ToListAsync();
        return data;
    }
}