namespace Common.Models;

public class LeakSensorData : SensorData
{
    public DateTime DCreated { get; set; }
    public DateTime DReported { get; set; }
    public int DLifeTimeUseCount { get; set; }
    public int LeakLevelId { get; set; }
    public double DTemperatureOut { get; set; }
    public double DTemperatureIn { get; set; }
}
