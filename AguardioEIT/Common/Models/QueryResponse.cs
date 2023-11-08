namespace Common.Models;

public class QueryResponse
{
    public bool FromCache { get; set; }
    public IEnumerable<SensorData?>? Data { get; set; }
}
