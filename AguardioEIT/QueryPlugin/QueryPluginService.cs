using Common.Enum;
using Common.Models;
using Interfaces;
using Newtonsoft.Json;

namespace QueryPlugin;

public class QueryPluginService : IQueryPluginService
{
    private readonly IRedisPluginService _redisService;
    private readonly ILeakSensorMongoDatabasePluginService _leakSensorMongoDatabasePluginService;
    private readonly ILeakSensorSqlDatabasePluginService _leakSensorSqlDatabasePluginService;

    public QueryPluginService(
        IRedisPluginService redisService,
        ILeakSensorMongoDatabasePluginService leakSensorMongoDatabasePluginService,
        ILeakSensorSqlDatabasePluginService leakSensorSqlDatabasePluginService
    )
    {
        _redisService = redisService;
        _leakSensorMongoDatabasePluginService = leakSensorMongoDatabasePluginService;
        _leakSensorSqlDatabasePluginService = leakSensorSqlDatabasePluginService;
    }
    
    /// <summary>
    /// Performs a lookup in the cache for the given cacheKey. If the data is found, it is returned.
    /// Else the data is fetched from the database, cached and returned.
    /// </summary>
    /// <param name="query">The query to be performed if no data is found in the cache</param>
    /// <param name="queryId">The query to use in a database lookup. Can be a dataId or a sensorId</param>
    /// <returns></returns>
    public async Task<QueryResponse> GetStoredData(Query query, int queryId)
    {
        try
        {
            // Throws KeyNotFoundException no data is found
            return await FetchDataFromCache(query, queryId);
        }
        catch (KeyNotFoundException)
        {
            return await FetchDataFromDb(query, queryId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            
            return new QueryResponse
            {
                fromCache = false,
                data = null
            };
        }
    }

    private async Task<QueryResponse> FetchDataFromCache(Query query, int queryId)
    {
        string cacheKey = GetCacheKey(query, queryId);
        string cachedData = await _redisService.GetAsync(cacheKey);

        IEnumerable<LeakSensorData>? data = JsonConvert.DeserializeObject<List<LeakSensorData>>(cachedData);
        
        return new QueryResponse
        {
            fromCache = true,
            data = data
        };
    }

    private async Task<QueryResponse> FetchDataFromDb(Query query, int queryId)
    {
        IEnumerable<LeakSensorData>? data = query switch
        {
            Query.SqlGetByDataId => new List<LeakSensorData>
            {
                await _leakSensorSqlDatabasePluginService.GetSensorDataByIdAsync(queryId)
            },
            Query.SqlGetBySensorId => await _leakSensorSqlDatabasePluginService.GetSensorDataBySensorIdAsync(queryId),
            Query.MongoDbGetByDataId => new List<LeakSensorData>
            {
                await _leakSensorMongoDatabasePluginService.GetSensorDataByIdAsync(queryId)
            },
            Query.MongoDbGetBySensorId => await _leakSensorMongoDatabasePluginService.GetSensorDataBySensorIdAsync(queryId),
            _ => throw new ArgumentOutOfRangeException(nameof(query), query, null)
        };

        if (data is null) throw new Exception("No data found in database");
        
        // Set data in cache with 30 minutes expiration
        string serializedData = JsonConvert.SerializeObject(data);
        string cacheKey = GetCacheKey(query, queryId);
        await _redisService.SetAsync(cacheKey, serializedData);
        
        return new QueryResponse
        {
            fromCache = false,
            data = data
        };
    }
    
    private static string GetCacheKey(Query query, int queryId)
    {
        string cacheKey = query switch
        {
            Query.SqlGetByDataId => $"Sql:DataId={queryId}",
            Query.SqlGetBySensorId => $"Sql:SensorId={queryId}",
            Query.MongoDbGetByDataId => $"MongoDb:DataId={queryId}",
            Query.MongoDbGetBySensorId => $"MongoDb:SensorId={queryId}",
            _ => throw new ArgumentOutOfRangeException(nameof(query), query, null)
        };
        return cacheKey;
    }
}
