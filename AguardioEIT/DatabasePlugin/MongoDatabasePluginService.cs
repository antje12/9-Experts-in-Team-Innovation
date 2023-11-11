using Common.Models;
using DatabasePlugin.Context;
using Interfaces;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace DatabasePlugin;

public sealed class MongoDatabasePluginService : IMongoDatabasePluginService
{
    private readonly MongoDbContext _mongoDbContext;
    private readonly IRedisPluginService _redisPluginService;

    public MongoDatabasePluginService(MongoDbContext mongoDbContext, IRedisPluginService redisPluginService) {
        _mongoDbContext = mongoDbContext ?? throw new ArgumentNullException(nameof(mongoDbContext));
        _redisPluginService = redisPluginService ?? throw new ArgumentNullException(nameof(redisPluginService));
    }

    public async Task SaveSensorDataAsync<T>(IEnumerable<T> data) where T : SensorData
    {
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IEnumerable<T> sensorData = data.ToList();
        
        await collection.InsertManyAsync(sensorData);
        
        foreach (T d in sensorData)
        {
            await _redisPluginService.SetAsync($"MongoDb:{typeof(T).Name}:DataId={d.DataRawId}", JsonConvert.SerializeObject(data));
        }

        IEnumerable<T> sensorDataCollection = await GetSensorDataBySensorIdAsync<T>(sensorData.First().SensorId);
        await _redisPluginService.SetAsync($"MongoDb:SensorId={sensorData.First().SensorId}", JsonConvert.SerializeObject(sensorDataCollection));
    }

    public async Task<T?> GetSensorDataByIdAsync<T>(int dataId) where T : SensorData
    {
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IMongoQueryable<T> query = collection.AsQueryable().Where(x => x.DataRawId == dataId);
        T? data = await query.FirstOrDefaultAsync();
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return data;
    }

    public async Task<IEnumerable<T>> GetSensorDataBySensorIdAsync<T>(int sensorId) where T : SensorData
    {
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IMongoQueryable<T> query = collection.AsQueryable().Where(x => x.SensorId == sensorId);
        List<T> data = await query.ToListAsync();
        return data;
    }
}
