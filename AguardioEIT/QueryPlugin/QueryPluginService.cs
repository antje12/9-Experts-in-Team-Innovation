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
    /// <param name="cacheKey">The key used to perform a lookup in the cache</param>
    /// <param name="query">The query to be performed if no data is found in the cache</param>
    /// <param name="queryId">The query to use in a database lookup. Can be a dataId or a sensorId</param>
    /// <returns></returns>
    public async Task<QueryResponse> GetStoredData(string cacheKey, Query query, int queryId)
    {
        try
        {
            // Throws KeyNotFoundException no data is found
            return await FetchDataFromCache(cacheKey);
        }
        catch (KeyNotFoundException)
        {
            return await FetchDataFromDb(cacheKey, query, queryId);
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

    private async Task<QueryResponse> FetchDataFromCache(string cacheKey)
    {
        string cachedData = await _redisService.GetAsync(cacheKey);

        IEnumerable<LeakSensorData>? data = JsonConvert.DeserializeObject<List<LeakSensorData>>(cachedData);
        
        return new QueryResponse
        {
            fromCache = true,
            data = data
        };
    }

    private async Task<QueryResponse> FetchDataFromDb(string cacheKey, Query query, int queryId)
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
        await _redisService.SetAsync(cacheKey, serializedData, 30 * 60);
        
        return new QueryResponse
        {
            fromCache = false,
            data = data
        };
    }
}
