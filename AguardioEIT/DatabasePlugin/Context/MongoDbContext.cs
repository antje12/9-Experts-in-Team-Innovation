using Common.Models;
using MongoDB.Driver;

namespace DatabasePlugin.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        MongoClient client = new("mongodb://192.168.8.13:27017,192.168.8.12:27017,192.168.8.10:27017/?replicaSet=dbrs");
        _database = client.GetDatabase("aguardio");
    }

    public IMongoCollection<LeakSensorData> LeakSensorData => 
        _database.GetCollection<LeakSensorData>("LeakSensorData");
    public IMongoCollection<ShowerSensorData> ShowerSensorData => 
        _database.GetCollection<ShowerSensorData>("ShowerSensorData");

    public IMongoCollection<T> GetCollection<T>() where T : SensorData
    {
        string collectionName = typeof(T).Name;
        return _database.GetCollection<T>(collectionName);
    }
}
