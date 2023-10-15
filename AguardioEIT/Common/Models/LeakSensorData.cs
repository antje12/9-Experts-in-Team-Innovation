namespace DatabasePlugin.Models;

public class LeakSensorData
{
    public int DataRawId { get; set; }
    public DateTime DCreated { get; set; }
    public DateTime DReported { get; set; }
    public int DLifeTimeUseCount { get; set; }
    public int LeakLevelId { get; set; }
    public int SensorId { get; set; }
    public double DTemperatureOut { get; set; }
    public double DTemperatureIn { get; set; }
}
