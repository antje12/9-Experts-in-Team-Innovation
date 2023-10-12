namespace Interfaces;

public interface IRedisPluginService
{
    Task SetAsync(string key, string value, int? expirationSeconds = null);
    Task<string> GetAsync(string key);
}
