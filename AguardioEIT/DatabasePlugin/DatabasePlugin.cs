using Common.Models;
using DatabasePlugin.Context;
using DatabasePlugin.Interfaces;
using DatabasePlugin.Repositories;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatabasePlugin;

public class DatabasePlugin : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        const string sqlConnectionString = "Host=192.168.8.10;Database=aguardio;Username=postgres;Password=postgres;Port=5432";

        services.AddDbContext<SqlDbContext>(options =>
        {
            options.UseNpgsql(sqlConnectionString);
        });
        
        services.AddScoped<MongoDbContext>();
        services.AddScoped<ISqlDatabasePluginService, SqlDatabasePluginService>();
        services.AddScoped<IMongoDatabasePluginService, MongoDatabasePluginService>();
        services.AddScoped<ISensorDataRepository, SensorDataRepository>();
    }
}
