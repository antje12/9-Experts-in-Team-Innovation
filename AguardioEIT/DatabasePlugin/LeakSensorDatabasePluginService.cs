using DatabasePlugin.Enums;
using DatabasePlugin.Factories;
using DatabasePlugin.Models;
using DatabasePlugin.Repositories;
using Interfaces;

namespace DatabasePlugin;

public class LeakSensorDatabasePluginService : ILeakSensorDatabasePluginService
{
    private readonly ILeakSensorDataRepository _sqlRepository;
    private readonly ILeakSensorDataRepository _mongoRepository;
    public LeakSensorDatabasePluginService(IDbContextFactory dbContextFactory)
    {
        _sqlRepository = new LeakSensorDataRepository(
            dbContextFactory.CreateDbContext(
                DatabaseType.Sql,
                "Host=localhost;Database=aguardio;Username=postgres;Password=postgres"
            )
        );
        _mongoRepository = new LeakSensorDataRepository(
            dbContextFactory.CreateDbContext(
                DatabaseType.MongoDb,
                "mongodb://localhost:27017"
            )
        );
    }

    private ILeakSensorDataRepository GetRepository(DatabaseType databaseType)
    {
        return databaseType switch
        {
            DatabaseType.Sql => _sqlRepository,
            DatabaseType.MongoDb => _mongoRepository,
            _ => throw new NotSupportedException("DatabaseType is not supported")
        };
    }

    public async Task SaveSensorDataAsync(LeakSensorData data, DatabaseType databaseType)
    {
        ILeakSensorDataRepository repo = GetRepository(databaseType);
        await repo.AddAsync(data);
    }

    public async Task<LeakSensorData> GetSensorDataByIdAsync(int dataId, DatabaseType databaseType)
    {
        ILeakSensorDataRepository repo = GetRepository(databaseType);
        return await repo.GetByDataIdAsync(dataId);
    }

    public async Task<IEnumerable<LeakSensorData>> GetSensorDataBySensorIdAsync(int sensorId, DatabaseType databaseType)
    {
        ILeakSensorDataRepository repo = GetRepository(databaseType);
        return await repo.GetBySensorIdAsync(sensorId);
    }
}
