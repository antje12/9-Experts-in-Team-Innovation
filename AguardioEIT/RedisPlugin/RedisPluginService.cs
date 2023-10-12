using StackExchange.Redis;
using Interfaces;

namespace RedisPlugin;

public sealed partial class RedisPluginService : IRedisPluginService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;
    // Set to 30 minutes, because that's the update interval for the sensors
    private const int SlidingExpirationSeconds = 30 * 60; 
        
    public RedisPluginService()
    {
        _redis = ConnectionMultiplexer.Connect("localhost:6379");
        _db = _redis.GetDatabase();
    }

    public async Task SetAsync(string key, string value, int? expirationSeconds = null)
    {
        TimeSpan? expiration = null;
        if (expirationSeconds.HasValue)
            expiration = TimeSpan.FromSeconds(expirationSeconds.Value);

        await _db.StringSetAsync(key, value, expiration);
    }

    public async Task<string> GetAsync(string key)
    {
        RedisValue value = await _db.StringGetAsync(key);

        // Extend the expiration of the key if it exists
        if (value.HasValue)
            await _db.KeyExpireAsync(key, TimeSpan.FromSeconds(SlidingExpirationSeconds));

        if (value.IsNullOrEmpty)
            throw new KeyNotFoundException($"No value found for key: {key}");

        return value.ToString();
    }
}
