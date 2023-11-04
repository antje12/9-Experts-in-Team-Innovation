namespace Common.Models;

public class LeakSensorDataSimple
{
    public int DataRawId { get; set; }
    public string DCreated { get; set; }
    public string DReported { get; set; }
    public int DLifeTimeUseCount { get; set; }
    public int LeakLevelId { get; set; }
    public int SensorId { get; set; }
    public float DTemperatureOut { get; set; }
    public float DTemperatureIn { get; set; }
}