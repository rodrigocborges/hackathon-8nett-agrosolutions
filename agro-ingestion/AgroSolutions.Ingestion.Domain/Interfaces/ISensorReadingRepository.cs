using AgroSolutions.Ingestion.Domain.Entities;

namespace AgroSolutions.Ingestion.Domain.Interfaces;

public interface ISensorReadingRepository
{
    Task AddAsync(SensorReading reading);
}