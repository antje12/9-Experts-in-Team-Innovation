using System.ComponentModel.DataAnnotations;
using Common.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models;

public class LeakSensorData
{
    [Key]
    [BsonId(IdGenerator = typeof(Int32IdGenerator))]
    [BsonRepresentation(BsonType.Int32)]
    public int DataRawId { get; set; }
    public DateTime DCreated { get; set; }
    public DateTime DReported { get; set; }
    public int DLifeTimeUseCount { get; set; }
    public int LeakLevelId { get; set; }
    public int SensorId { get; set; }
    public double DTemperatureOut { get; set; }
    public double DTemperatureIn { get; set; }
}
