using Common.Enum;
using Common.Models;
using DatabasePlugin.Repositories;
using Interfaces;
using Newtonsoft.Json;

namespace DatabasePlugin;

public sealed class SqlDatabasePluginService : ISqlDatabasePluginService
{
    private readonly IRedisPluginService _redisPluginService;
    private readonly ISensorDataRepository _sensorDataRepository;

    public SqlDatabasePluginService(
        IRedisPluginService redisPluginService,
        ISensorDataRepository sensorDataRepository
    ) {
        _redisPluginService = redisPluginService ?? throw new ArgumentNullException(nameof(redisPluginService));
        _sensorDataRepository = sensorDataRepository ?? throw new ArgumentNullException(nameof(sensorDataRepository));
    }

    public async Task<long> SaveSensorDataAsync<T>(IEnumerable<T> data, SensorType sensorType) where T : SensorData
    {
        try
        {
            IEnumerable<T> sensorData = data.ToList();
            long insertTime = await _sensorDataRepository.AddDataAsync(sensorData, sensorType);
            
            // foreach (T d in sensorData)
            // {
            //     string cacheKeyDataId = $"SqlDb:{typeof(T).Name}:DataId={d.DataRawId}";
            //     await _redisPluginService.SetAsync(cacheKeyDataId, JsonConvert.SerializeObject(data));
            // }
            //
            // string cacheKeySensorId = $"SqlDb:{typeof(T).Name}:SensorId={sensorData.First().SensorId}";
            // QueryResponse<T> sensorDataCollection = await GetSensorDataBySensorIdAsync<T>(sensorData.First().SensorId, sensorType);
            // await _redisPluginService.SetAsync(cacheKeySensorId, JsonConvert.SerializeObject(sensorDataCollection.Data));

            return insertTime;
        } catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<QueryResponse<T>> GetSensorDataByIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData
    {
        return await _sensorDataRepository.GetByDataIdAsync<T>(dataId, sensorType);
    }

    public async Task<QueryResponse<T>> GetSensorDataBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData
    {
        return await _sensorDataRepository.GetBySensorIdAsync<T>(sensorId, sensorType);
    }
}
