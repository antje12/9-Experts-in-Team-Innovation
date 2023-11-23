using System.ComponentModel.DataAnnotations;
using Common.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Models;

public class ShowerSensorDataSimple
{
    public int DataRawId { get; set; }
    public string DCreated { get; set; }
    public string DReported { get; set; }
    public int SensorId { get; set; }
    public string DShowerState { get; set; }
    public string DTemperature { get; set; }
    public string DHumidity { get; set; }
    public string DBattery { get; set; }
}