using DatabasePlugin.Enums;
using Microsoft.EntityFrameworkCore;

namespace DatabasePlugin.Factories;

public interface IDbContextFactory
{
    DbContext CreateDbContext(DatabaseType dbType, string connectionString);
}
