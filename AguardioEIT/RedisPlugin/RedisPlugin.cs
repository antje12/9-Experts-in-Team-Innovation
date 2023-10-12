using Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RedisPlugin;

public class RedisPlugin : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        services.AddSingleton<IRedisPluginService, RedisPluginService>();
    }
}
