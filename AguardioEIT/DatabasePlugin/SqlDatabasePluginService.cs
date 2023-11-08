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

    public async Task SaveSensorDataAsync<T>(T data, SensorType sensorType) where T : SensorData
    {
        await _sensorDataRepository.AddDataAsync(data, sensorType);
        
        string cacheKeyDataId = $"SqlDb:{typeof(T).Name}:DataId={data.DataRawId}";
        string cacheKeySensorId = $"SqlDb:{typeof(T).Name}:SensorId={data.SensorId}";
        
        await _redisPluginService.SetAsync(cacheKeyDataId, JsonConvert.SerializeObject(data));
        IEnumerable<SensorData> sensorDataCollection = await GetSensorDataBySensorIdAsync<T>(data.SensorId, sensorType);
        await _redisPluginService.SetAsync(cacheKeySensorId, JsonConvert.SerializeObject(sensorDataCollection));
    }

    public async Task<SensorData?> GetSensorDataByIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData
    {
        return await _sensorDataRepository.GetByDataIdAsync<T>(dataId, sensorType);
    }

    public async Task<IEnumerable<SensorData>> GetSensorDataBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData
    {
        return await _sensorDataRepository.GetBySensorIdAsync<T>(sensorId, sensorType);
    }
}
