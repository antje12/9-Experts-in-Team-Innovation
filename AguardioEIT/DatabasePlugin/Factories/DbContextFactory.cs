using DatabasePlugin.Context;
using DatabasePlugin.Enums;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace DatabasePlugin.Factories;

public class DbContextFactory : IDbContextFactory
{
    public DbContext CreateDbContext(DatabaseType dbType, string connectionString)
    {
        DbContextOptionsBuilder<DbContext> optionsBuilder = new();
        switch (dbType)
        {
            case DatabaseType.Sql:
                optionsBuilder.UseNpgsql(connectionString);
                return new SqlDbContext(optionsBuilder.Options);
            case DatabaseType.MongoDb:
                MongoClient mongoClient = new(connectionString);
                return MongoDbContext.Create(mongoClient.GetDatabase("aguardio"));
            default:
                throw new NotSupportedException($"DatabaseType {dbType} is not supported");
        }
    }
}
