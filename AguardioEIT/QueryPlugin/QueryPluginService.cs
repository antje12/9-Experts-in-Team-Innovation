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
    public async Task<QueryResponse> GetStoredData<T>(Query query, int queryId, SensorType sensorType) where T : SensorData
    {
        try
        {
            // Throws KeyNotFoundException no data is found
            return await FetchDataFromCache<T>(query, queryId);
        }
        catch (KeyNotFoundException)
        {
            return await FetchDataFromDb<T>(query, queryId, sensorType);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            return new QueryResponse
            {
                FromCache = false,
                Data = null
            };
        }
    }

    private async Task<QueryResponse> FetchDataFromCache<T>(Query query, int queryId) where T : SensorData
    {
        string cacheKey = GetCacheKey<T>(query, queryId);
        string cachedData = await _redisService.GetAsync(cacheKey);

        IEnumerable<LeakSensorData>? data = JsonConvert.DeserializeObject<List<LeakSensorData>>(cachedData);
        
        return new QueryResponse
        {
            FromCache = true,
            Data = data
        };
    }

    private async Task<QueryResponse> FetchDataFromDb<T>(Query query, int queryId, SensorType sensorType) where T : SensorData
    {
        IEnumerable<SensorData> data = query switch
        {
            Query.SqlGetByDataId => new List<SensorData?>
            {
                await _sqlDatabasePluginService.GetSensorDataByIdAsync<T>(queryId, sensorType)
            }!,
            Query.SqlGetBySensorId => await _sqlDatabasePluginService.GetSensorDataBySensorIdAsync<T>(queryId, sensorType),
            Query.MongoDbGetByDataId => new List<SensorData?>
            {
                await _mongoDatabasePluginService.GetSensorDataByIdAsync<T>(queryId)
            }!,
            Query.MongoDbGetBySensorId => await _mongoDatabasePluginService.GetSensorDataBySensorIdAsync<T>(queryId),
            _ => throw new ArgumentOutOfRangeException(nameof(query), query, null)
        };

        if (data is null) throw new Exception("No data found in database");
        
        // Set data in cache with 30 minutes expiration
        string serializedData = JsonConvert.SerializeObject(data);
        string cacheKey = GetCacheKey<T>(query, queryId);
        await _redisService.SetAsync(cacheKey, serializedData);
        
        return new QueryResponse
        {
            FromCache = false,
            Data = data
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
