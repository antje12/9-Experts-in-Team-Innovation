using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DatabasePlugin.Context;

public class SqlDbContextFactory : IDesignTimeDbContextFactory<SqlDbContext>
{
    public SqlDbContext CreateDbContext(string[]? args = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlDbContext>();
        optionsBuilder.UseNpgsql("Host=localhost;Database=aguardio;Username=postgres;Password=postgres;Port=5444");

        return new SqlDbContext(optionsBuilder.Options);
    }
}