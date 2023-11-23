using System.Diagnostics;
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

    public async Task<long> AddDataAsync(IEnumerable<SensorData> data, SensorType sensorType)
    {
        Stopwatch stopwatch = new();
        
        List<SensorData> sensorData = data.ToList();
        if (sensorData.Count == 0) throw new ArgumentException("Cannot insert empty list");
        switch (sensorType)
        {
            case SensorType.LeakSensor:
                if (sensorData.Count > 1)
                {
                    List<LeakSensorData> leakSensorData = sensorData.ConvertAll(d => (LeakSensorData)d);
                    stopwatch.Start();
                    await _leakDbSet.AddRangeAsync(leakSensorData);
                }
                else
                {
                    stopwatch.Start();
                    await  _leakDbSet.AddAsync((LeakSensorData)sensorData.First());
                }
                break;
            case SensorType.ShowerSensor:
                if (sensorData.Count > 1)
                {
                    List<ShowerSensorData> showerSensorData = sensorData.ConvertAll(d => (ShowerSensorData)d);
                    stopwatch.Start();
                    await _showerDbSet.AddRangeAsync(showerSensorData);
                }
                else
                {
                    stopwatch.Start();
                    await  _showerDbSet.AddAsync((ShowerSensorData)sensorData.First());
                }
                break;
            default:
                throw new ArgumentException("Invalid sensor type.");
        }
        
        await _context.SaveChangesAsync();
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }

    public async Task<QueryResponse<SensorData>> GetByDataIdAsync<T>(int dataId, SensorType sensorType) where T : SensorData
    {
        Stopwatch sw = new();
        
        sw.Start();
        SensorData? data = sensorType switch
        {
            SensorType.LeakSensor => await _leakDbSet.FindAsync(dataId),
            SensorType.ShowerSensor => await _showerDbSet.FindAsync(dataId),
            _ => throw new ArgumentException("Invalid sensor type.")
        };
        sw.Stop();

        if (data is null) throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
        
        List<SensorData> sensorDataList = new() { data };
        return new QueryResponse<SensorData>
        {
            Data = sensorDataList,
            FetchedItems = sensorDataList.Count,
            FromCache = false,
            QueryTimeMs = sw.ElapsedMilliseconds
        };
    }

    public async Task<QueryResponse<SensorData>> GetBySensorIdAsync<T>(int sensorId, SensorType sensorType) where T : SensorData
    {
        Stopwatch sw = new();
        
        sw.Start();
        IEnumerable<SensorData> data = sensorType switch
        {
            SensorType.LeakSensor => await _leakDbSet.Where(data => data.SensorId == sensorId).ToListAsync(),
            SensorType.ShowerSensor => await _showerDbSet.Where(data => data.SensorId == sensorId).ToListAsync(),
            _ => throw new ArgumentException("Invalid sensor type.")
        };
        sw.Stop();

        List<SensorData> sensorDataList = data.ToList();
        return new QueryResponse<SensorData>
        {
            Data = sensorDataList,
            FetchedItems = sensorDataList.Count,
            FromCache = false,
            QueryTimeMs = sw.ElapsedMilliseconds
        };
    }
}
