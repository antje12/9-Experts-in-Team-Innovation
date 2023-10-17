using Common.Models;
using DatabasePlugin.Context;
using Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DatabasePlugin;

public sealed class LeakSensorMongoDatabasePluginService : ILeakSensorMongoDatabasePluginService
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly IRedisPluginService _redisPluginService;

    public LeakSensorMongoDatabasePluginService(MongoDbContext mongoDbContext, IRedisPluginService redisPluginService)
    {
        _mongoDbContext = mongoDbContext;
        _redisPluginService = redisPluginService;
    }
    
    public async Task SaveSensorDataAsync(LeakSensorData data)
    {
        await _mongoDbContext.LeakSensorDatas.InsertOneAsync(data);
        await _redisPluginService.SetAsync($"MongoDb:DataId={data.DataRawId}", JsonConvert.SerializeObject(data));

        IEnumerable<LeakSensorData> sensorDataCollection = await GetSensorDataBySensorIdAsync(data.SensorId);
        await _redisPluginService.SetAsync($"MongoDb:SensorId={data.SensorId}", JsonConvert.SerializeObject(sensorDataCollection));
    }

    public async Task<LeakSensorData> GetSensorDataByIdAsync(int dataId)
    {
        IMongoQueryable<LeakSensorData>? query = _mongoDbContext.LeakSensorDatas.AsQueryable().Where(x => x.DataRawId == dataId);
        LeakSensorData? data = await query.FirstOrDefaultAsync();
    
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return data;
    }

    public async Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId)
    {
        IMongoQueryable<LeakSensorData> query = _mongoDbContext.LeakSensorDatas.AsQueryable().Where(x => x.SensorId == sensorId);
        List<LeakSensorData> data = await query.ToListAsync();
        return data;
    }
}
