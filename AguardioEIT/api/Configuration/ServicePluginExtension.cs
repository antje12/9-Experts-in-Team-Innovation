using System.Reflection;
using Interfaces;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Runtime.Loader;

namespace api.Configuration
{
    public static class ServicePluginExtension
    {
        public static IServiceCollection LoadPlugins(this IServiceCollection services)
        {
            string pluginsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            string[] pluginFiles = Directory.GetFiles(pluginsDirectory, "*.dll");

            AssemblyLoadContext loadContext = new(null, true);
            loadContext.Resolving += (context, name) =>
            {
                string dependencyPath = Path.Combine(pluginsDirectory, $"{name.Name}.dll");
                return context.LoadFromAssemblyPath(dependencyPath);
            };

            foreach (string pluginFile in pluginFiles)
            {
                Console.WriteLine($"Attempting to load assembly from: {pluginFile}");
                try
                {
                    Assembly assembly = loadContext.LoadFromAssemblyPath(pluginFile);

                    Type? pluginClass = assembly.GetTypes().FirstOrDefault(t => typeof(IPlugin).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                    if (pluginClass == null) continue;

                    AssemblyPart part = new(assembly);

                    services.AddControllersWithViews().ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));

                    MethodInfo? initMethod = pluginClass.GetMethod(nameof(IPlugin.Initialize), BindingFlags.Public | BindingFlags.Instance);
                    object? obj = Activator.CreateInstance(pluginClass);
                    initMethod.Invoke(obj, new object[] { services });
                }
                catch (BadImageFormatException)
                {
                    Console.WriteLine($"Error loading {pluginFile}: Not a valid .NET assembly or targets a different runtime.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading {pluginFile}: {ex.Message}");
                }
            }

            return services;
        }
    }
}
