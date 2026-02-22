using AgroSolutions.Ingestion.Domain.Entities;
using AgroSolutions.Ingestion.Domain.Interfaces;
using AgroSolutions.Ingestion.Infrastructure.DatabaseContext;

namespace AgroSolutions.Ingestion.Infrastructure.Repositories;

public class SensorReadingRepository : ISensorReadingRepository
{
    private readonly AppDbContext _context;

    public SensorReadingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(SensorReading reading)
    {
        await _context.SensorReadings.AddAsync(reading);
        await _context.SaveChangesAsync();
    }
}