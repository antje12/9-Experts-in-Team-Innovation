using DatabasePlugin.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DatabasePlugin;

public class SensorDataServiceFactory : ISensorDataServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SensorDataServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISensorDataService<T> GetService<T>()
    {
        return _serviceProvider.GetRequiredService<ISensorDataService<T>>();
    }
}
