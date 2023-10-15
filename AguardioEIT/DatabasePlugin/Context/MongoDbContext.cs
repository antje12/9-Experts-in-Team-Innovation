using Common.Models;
using MongoDB.Driver;

namespace DatabasePlugin.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        MongoClient client = new("mongodb://localhost:20000,localhost:20001,localhost:20002/?replicaSet=dbrs");
        _database = client.GetDatabase("aguardio");
    }

    public IMongoCollection<LeakSensorData> LeakSensorDatas => 
        _database.GetCollection<LeakSensorData>("LeakSensorData");
}
