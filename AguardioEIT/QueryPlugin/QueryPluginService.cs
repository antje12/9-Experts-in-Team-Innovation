using Common.Enum;
using Common.Models;
using Interfaces;
using Newtonsoft.Json;

namespace QueryPlugin;

public class QueryPluginService : IQueryPluginService
{
    private readonly IRedisPluginService _redisService;
    private readonly IMongoDatabasePluginService _mongoDatabasePluginService;
    private readonly ISqlDatabasePluginService _sqlDatabasePluginService;

    public QueryPluginService(
        IRedisPluginService redisService,
        IMongoDatabasePluginService mongoDatabasePluginService,
        ISqlDatabasePluginService sqlDatabasePluginService
    )
    {
        _redisService = redisService;
        _mongoDatabasePluginService = mongoDatabasePluginService;
        _sqlDatabasePluginService = sqlDatabasePluginService;
    }

    /// <summary>
    /// Performs a lookup in the cache for the given cacheKey. If the data is found, it is returned.
    /// Else the data is fetched from the database, cached and returned.
    /// </summary>
    /// <param name="query">The query to be performed if no data is found in the cache</param>
    /// <param name="queryId">The query to use in a database lookup. Can be a dataId or a sensorId</param>
    /// <param name="sensorType">The <see cref="SensorType"/> that should be queried</param>
    /// <returns></returns>
    public async Task<QueryResponse<TU>> GetStoredData<T, TU>(Query query, int queryId, SensorType sensorType) where T : SensorData where TU : SensorData
    {
        try
        {
            return await FetchDataFromCache<TU>(query, queryId);
        }
        catch (KeyNotFoundException)
        {
            return await FetchDataFromDb<TU>(query, queryId, sensorType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            return new QueryResponse<TU>
            {
                FromCache = false,
                Data = null
            };
        }
    }

    private async Task<QueryResponse<T>> FetchDataFromCache<T>(Query query, int queryId) where T : SensorData
    {
        string cacheKey = GetCacheKey<T>(query, queryId);
        string cachedData = await _redisService.GetAsync(cacheKey);

        IEnumerable<T>? data = JsonConvert.DeserializeObject<List<T>>(cachedData);
        
        return new QueryResponse<T>
        {
            FromCache = true,
            Data = data
        };
    }

    private async Task<QueryResponse<T>> FetchDataFromDb<T>(Query query, int queryId, SensorType sensorType) where T : SensorData
    {
        QueryResponse<T> rawData = query switch
        {
            Query.SqlGetByDataId => await _sqlDatabasePluginService.GetSensorDataByIdAsync<T>(queryId, sensorType),
            Query.SqlGetBySensorId => await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<T>(queryId, sensorType),
            Query.MongoDbGetByDataId => await _mongoDatabasePluginService.GetSensorDataByIdAsync<T>(queryId),
            Query.MongoDbGetBySensorId => await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<T>(queryId),
            _ => throw new ArgumentOutOfRangeException(nameof(query), query, null)
        };

        if (rawData is null) throw new Exception("No data found in database");

        if (rawData.Data is null) throw new KeyNotFoundException($"Data with the id {queryId} was not found.");

        // Set data in cache with 30 minutes expiration
        string serializedData = JsonConvert.SerializeObject(rawData);
        string cacheKey = GetCacheKey<T>(query, queryId);
        await _redisService.SetAsync(cacheKey, serializedData);
    
        return new QueryResponse<T>
        {
            FromCache = false,
            Data = rawData.Data
        };
        
    }

    private static string GetCacheKey<T>(Query query, int queryId) where T : SensorData
    {
        string cacheKey = query switch
        {
            Query.SqlGetByDataId => $"Sql:{typeof(T).Name}:DataId={queryId}",
            Query.SqlGetBySensorId => $"Sql:{typeof(T).Name}:SensorId={queryId}",
            Query.MongoDbGetByDataId => $"MongoDb:{typeof(T).Name}:DataId={queryId}",
            Query.MongoDbGetBySensorId => $"MongoDb:{typeof(T).Name}:SensorId={queryId}",
            _ => throw new ArgumentOutOfRangeException(nameof(query), query, null)
        };
        return cacheKey;
    }
}
