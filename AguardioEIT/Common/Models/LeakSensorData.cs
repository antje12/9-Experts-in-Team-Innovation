using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models;

public class LeakSensorData
{
    [Key]
    [BsonId]
    [BsonRepresentation(BsonType.Int32)] // Using ObjectId type for MongoDB's _id
    [BsonIgnoreIfDefault]
    public int DataRawId { get; set; }
    public DateTime DCreated { get; set; }
    public DateTime DReported { get; set; }
    public int DLifeTimeUseCount { get; set; }
    public int LeakLevelId { get; set; }
    public int SensorId { get; set; }
    public double DTemperatureOut { get; set; }
    public double DTemperatureIn { get; set; }
}
