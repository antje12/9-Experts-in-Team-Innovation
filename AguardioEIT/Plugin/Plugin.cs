using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin
{
    public class Plugin : IPlugin
    {
        public void Initialize(IServiceCollection services)
        {
            services.AddSingleton<IPluginService,PluginService>();
        }
    }
}