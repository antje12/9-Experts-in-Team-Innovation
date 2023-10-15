using DatabasePlugin.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabasePlugin.Context;

public class SqlDbContext : DbContext
{
    public DbSet<LeakSensorData> LeakSensorData { get; set; }
    
    public SqlDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
