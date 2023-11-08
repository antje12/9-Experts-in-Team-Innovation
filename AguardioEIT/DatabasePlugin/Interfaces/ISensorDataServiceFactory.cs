namespace DatabasePlugin.Interfaces;

public interface ISensorDataServiceFactory
{
    ISensorDataService<T> GetService<T>();
}
