using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DatabasePlugin.Context;

public class SqlDbContextFactory : IDesignTimeDbContextFactory<SqlDbContext>
{
    public SqlDbContext CreateDbContext(string[]? args = null)
    {
        DbContextOptionsBuilder<SqlDbContext> optionsBuilder = new();
        optionsBuilder.UseNpgsql("Host=localhost;Database=aguardio;Username=postgres;Password=postgres;Port=5444");

        return new SqlDbContext(optionsBuilder.Options);
    }
}
