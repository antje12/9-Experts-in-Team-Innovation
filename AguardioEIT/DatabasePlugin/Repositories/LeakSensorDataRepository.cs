using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabasePlugin.Repositories;

public class LeakSensorDataRepository : ILeakSensorDataRepository
{
    private readonly DbContext _context;
    private readonly DbSet<LeakSensorData> _dbSet;
    
    public LeakSensorDataRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<LeakSensorData>();
    }
    
    public async Task AddAsync(LeakSensorData data)
    {
        await _dbSet.AddAsync(data);
        await _context.SaveChangesAsync();
    }

    public async Task<LeakSensorData> GetByDataIdAsync(int dataId)
    {
        return await _dbSet.FindAsync(dataId) ?? 
               throw new KeyNotFoundException($"Data with the id {dataId} was not found.");
    }

    public async Task<IEnumerable<LeakSensorData>> GetBySensorIdAsync(int sensorId)
    {
        return await _dbSet.Where(data => data.SensorId == sensorId).ToListAsync();
    }
}
