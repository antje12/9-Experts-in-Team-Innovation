using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabasePlugin.Context;

public class SqlDbContext : DbContext
{
    public DbSet<LeakSensorData> LeakSensorData { get; set; }
    
    public SqlDbContext(DbContextOptions<SqlDbContext> options)
        : base(options)
    {
    }
}
