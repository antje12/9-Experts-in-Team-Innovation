namespace Common.Models;

public class LeakSensorDataSimple
{
  public int DataRawId { get; set; }
  public string DCreated { get; set; }
  public string DReported { get; set; }
  public string DLifeTimeUseCount { get; set; }
  public int LeakLevelId { get; set; }
  public int SensorId { get; set; }
  public string DTemperatureOut { get; set; }
  public string DTemperatureIn { get; set; }
}