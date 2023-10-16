using Common.Models;
using DatabasePlugin.Context;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DatabasePlugin;

public class LeakSensorMongoDatabasePluginService
{
    private readonly MongoDbContext _mongoDbContext;

    public LeakSensorMongoDatabasePluginService(
        MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }
    
    public async Task SaveSensorDataAsync(LeakSensorData data)
    {
        await _mongoDbContext.LeakSensorDatas.InsertOneAsync(data);
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
