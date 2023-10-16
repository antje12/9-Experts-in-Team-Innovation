using DatabasePlugin.Context;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DatabasePlugin;

public class DatabasePlugin : IPlugin
{
    public void Initialize(IServiceCollection services)
    {
        const string sqlConnectionString = "Host=postgres;Database=aguardio;Username=postgres;Password=postgres;Port=5432";

        services.AddDbContext<SqlDbContext>(options =>
        {
            options.UseNpgsql(sqlConnectionString);
        });

        services.AddScoped<MongoDbContext>();
        services.AddScoped<LeakSensorSqlDatabasePluginService>();
        services.AddScoped<LeakSensorMongoDatabasePluginService>();
    }
}
