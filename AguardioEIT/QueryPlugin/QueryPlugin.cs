using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace QueryPlugin;

public class QueryPlugin : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        services.AddScoped<IQueryPluginService, QueryPluginService>();
    }
}
