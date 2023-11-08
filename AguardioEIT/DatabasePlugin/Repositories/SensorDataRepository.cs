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

    public async Task AddDataAsync(SensorData data, SensorType sensorType)
    {
        switch (sensorType)
        {
            case SensorType.LeakSensor:
                await  _leakDbSet.AddAsync((LeakSensorData)data);
                break;
            case SensorType.ShowerSensor:
                await _showerDbSet.AddAsync((ShowerSensorData)data);
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
