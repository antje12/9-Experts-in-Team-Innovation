using System.Diagnostics;
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

    public async Task<long> SaveSensorDataAsync<T>(IEnumerable<T> data) where T : SensorData
    {
        Stopwatch stopwatch = new();
        
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IEnumerable<T> sensorData = data.ToList();
        
        stopwatch.Start();
        await collection.InsertManyAsync(sensorData);
        stopwatch.Stop();
        
        foreach (T d in sensorData)
        {
            await _redisPluginService.SetAsync($"MongoDb:{typeof(T).Name}:DataId={d.DataRawId}", JsonConvert.SerializeObject(data));
        }

        IEnumerable<T>? sensorDataCollectionResponse = (await GetSensorDataBySensorIdAsync<T>(sensorData.First().SensorId)).Data;
        await _redisPluginService.SetAsync($"MongoDb:SensorId={sensorData.First().SensorId}", JsonConvert.SerializeObject(sensorDataCollectionResponse));

        return stopwatch.ElapsedMilliseconds;
    }

    public async Task<QueryResponse<T>> GetSensorDataByIdAsync<T>(int dataId) where T : SensorData
    {
        Stopwatch sw = new();
        
        sw.Start();
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IMongoQueryable<T> query = collection.AsQueryable().Where(x => x.DataRawId == dataId);
        IEnumerable<T>? data = new List<T> { await query.FirstOrDefaultAsync() };
        sw.Start();
        
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return new QueryResponse<T>
        {
            Data = data,
            FetchedItems = data.ToList().Count,
            FromCache = false,
            QueryTimeMs = sw.ElapsedMilliseconds
        };
    }

    public async Task<QueryResponse<T>> GetSensorDataBySensorIdAsync<T>(int sensorId) where T : SensorData
    {
        Stopwatch sw = new();
        
        sw.Start();
        IMongoCollection<T> collection = _mongoDbContext.GetCollection<T>();
        IMongoQueryable<T> query = collection.AsQueryable().Where(x => x.SensorId == sensorId);
        IEnumerable<T>? data = await query.ToListAsync();
        
        sw.Stop();
        return new QueryResponse<T>
        {
            Data = data,
            FetchedItems = data.ToList().Count,
            FromCache = false,
            QueryTimeMs = sw.ElapsedMilliseconds
        };
    }
}
