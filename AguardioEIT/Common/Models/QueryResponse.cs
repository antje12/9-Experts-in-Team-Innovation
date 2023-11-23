namespace Common.Models;

public class QueryResponse<T> where T : SensorData
{
    public bool FromCache { get; set; }
    public long QueryTimeMs { get; set; }
    public int FetchedItems { get; set; }
    public IEnumerable<T>? Data { get; set; }
}
