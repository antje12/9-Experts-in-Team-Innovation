using System.Reflection;
using Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Options;

namespace api;

public static class ServicePluginExtension
{
    public static IServiceCollection LoadPlugins(this IServiceCollection services, IConfiguration configuration)
    {
        var plugins = configuration.GetSection("Plugins").Get<List<PluginConfiguration>>();

        plugins.ForEach(p =>
        {
            Assembly assembly = Assembly.LoadFrom(p.Path);
            var part = new AssemblyPart(assembly);
            
            services.AddControllersWithViews().ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));

            var pluginClass = assembly.GetTypes().SingleOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));

            if (pluginClass != null)
            {
                var initMethod = pluginClass.GetMethod(nameof(IPlugin.Initialize), BindingFlags.Public | BindingFlags.Instance);
                var obj = Activator.CreateInstance(pluginClass);
                initMethod.Invoke(obj, new object[] { services });
            }
        });

        return services;
    }
}
