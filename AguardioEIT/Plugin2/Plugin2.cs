using Microsoft.Extensions.DependencyInjection;
using Plugin2;

namespace Interfaces;

public class Plugin2 : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        services.AddSingleton<IPlugin2Service, Plugin2Service>();
    }
}