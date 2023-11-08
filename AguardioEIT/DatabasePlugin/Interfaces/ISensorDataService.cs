namespace DatabasePlugin.Interfaces;

public interface ISensorDataService<T>
{
    Task SaveSensorDataAsync(T data);
    Task<T> GetSensorDataByIdAsync(int dataId);
    Task<IEnumerable<T>> GetSensorDataBySensorIdAsync(int sensorId);
}
