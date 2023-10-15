using DatabasePlugin.Models;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;

namespace DatabasePlugin.Context;

public class MongoDbContext : DbContext
{
    public DbSet<LeakSensorData> SensorData { get; set; }
    
    public static MongoDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<MongoDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);
    
    public MongoDbContext(DbContextOptions options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<LeakSensorData>().ToCollection("sensor_data");
    }
}