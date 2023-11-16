using System.ComponentModel.DataAnnotations;
using Common.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models;

public abstract class SensorData
{
    [Key]
    [BsonId(IdGenerator = typeof(Int32IdGenerator))]
    [BsonRepresentation(BsonType.Int32)]
    public int DataRawId { get; init; }
    public int SensorId { get; init; }
}