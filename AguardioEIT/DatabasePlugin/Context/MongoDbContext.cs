using Common.Models;
using MongoDB.Driver;

namespace DatabasePlugin.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext()
    {
        MongoClient client = new("mongodb://mongo.one.db:27017,mongo.two.db:27017,mongo.three.db:27017/?replicaSet=dbrs");
        _database = client.GetDatabase("aguardio");
    }

    public IMongoCollection<LeakSensorData> LeakSensorDatas => 
        _database.GetCollection<LeakSensorData>("LeakSensorData");
}
