using Common.Enum;
using Common.Models;
using DatabasePlugin.Context;
using Microsoft.EntityFrameworkCore;

namespace DatabasePlugin.Repositories;

public class SensorDataRepository : ISensorDataRepository
{
    private readonly SqlDbContext _context;
    private readonly DbSet<LeakSensorData> _leakDbSet;
    private readonly DbSet<ShowerSensorData> _showerDbSet;

    public SensorDataRepository(SqlDbContext context)
    {
        _context = context;
        _leakDbSet = context.Set<LeakSensorData>();
        _showerDbSet = context.Set<ShowerSensorData>();
    }

    public async Task AddDataAsync(IEnumerable<SensorData> data, SensorType sensorType)
    {
        List<SensorData> sensorData = data.ToList();
        if (sensorData.Count == 0) throw new ArgumentException("Cannot insert empty list");
        switch (sensorType)
        {
            case SensorType.LeakSensor:
                if (sensorData.Count > 1)
                {
                    List<LeakSensorData> leakSensorData = sensorData.ConvertAll(d => (LeakSensorData)d);
                    Console.WriteLine($"Adding {leakSensorData.Count} leak sensor data to the database.");
                    await _leakDbSet.AddRangeAsync(leakSensorData);
                }
                else
                {
                    await  _leakDbSet.AddAsync((LeakSensorData)sensorData.First());
                }
                break;
            case SensorType.ShowerSensor:
                if (sensorData.Count > 1)
                {
                    List<ShowerSensorData> showerSensorData = sensorData.ConvertAll(d => (ShowerSensorData)d);
                    await _showerDbSet.AddRangeAsync(showerSensorData);
                }
                else
                {
                    await  _showerDbSet.AddAsync((ShowerSensorData)sensorData.First());
                }
                break;
            default:
                throw new ArgumentException("Invalid sensor type.");
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task<SensorData> GetByDataIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData
    {
        SensorData? data = sensorType switch
        {
            SensorType.LeakSensor => await _leakDbSet.FindAsync(dataId),
            SensorType.ShowerSensor => await _showerDbSet.FindAsync(dataId),
            _ => throw new ArgumentException("Invalid sensor type.")
        };

        return data ?? throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
    }

    public async Task<IEnumerable<SensorData>> GetBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData
    {
        IEnumerable<SensorData> data = sensorType switch
        {
            SensorType.LeakSensor => await _leakDbSet.Where(data => data.SensorId == sensorId).ToListAsync(),
            SensorType.ShowerSensor => await _showerDbSet.Where(data => data.SensorId == sensorId).ToListAsync(),
            _ => throw new ArgumentException("Invalid sensor type.")
        };

        return data;
    }
}
