namespace Common.Models;

public class ShowerSensorData : SensorData
{
    public DateTime DCreated { get; set; }
    public DateTime DReported { get; set; }
    public int DShowerState { get; set; }
    public float DTemperature { get; set; }
    public int DHumidity { get; set; }
    public int DBattery { get; set; }
}
