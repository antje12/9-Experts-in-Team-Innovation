using Common.Models;
using DatabasePlugin.Context;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

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
        LeakSensorData? data = await _mongoDbContext.LeakSensorDatas
            .AsQueryable()
            .Where(x => x.DataRawId == dataId)
            .FirstOrDefaultAsync();
        
        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        return data;
    }

    public async Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId)
    {
        List<LeakSensorData> data = await _mongoDbContext.LeakSensorDatas
            .AsQueryable()
            .Where(x => x.SensorId == sensorId)
            .ToListAsync();

        return data;
    }
}
