using System.Reflection;
using Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace api.Configuration;

public static class ServicePluginExtension
{
    public static void LoadPlugins(this IServiceCollection services, IConfiguration configuration)
    {
        ResolvePlugins(services, configuration);
    }

    private static void ResolvePlugins(this IServiceCollection services, IConfiguration configuration)
    {
        List<PluginConfiguration>? plugins = Environment.GetEnvironmentVariable("DOCKERIZED") is null ? 
            configuration.GetSection("LocalPlugins").Get<List<PluginConfiguration>>() : 
            configuration.GetSection("DockerPlugins").Get<List<PluginConfiguration>>();
        
        plugins?.ForEach(p =>
        {
            Assembly assembly = Assembly.LoadFrom(p.Path);
            AssemblyPart part = new(assembly);
            
            services.AddControllersWithViews().ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));

            Type? pluginClass = assembly.GetTypes().SingleOrDefault(t => typeof(IPlugin).IsAssignableFrom(t));

            if (pluginClass == null) return;
            MethodInfo? initMethod = pluginClass.GetMethod(nameof(IPlugin.Initialize), BindingFlags.Public | BindingFlags.Instance);
            object? obj = Activator.CreateInstance(pluginClass);
            initMethod?.Invoke(obj, new object[] { services });
        });
    }
}
