namespace Common.Models;

public class QueryResponse
{
    public bool fromCache { get; set; }
    public IEnumerable<LeakSensorData>? data { get; set; }
}
