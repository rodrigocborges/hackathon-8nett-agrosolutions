using AgroSolutions.Ingestion.Application.DTOs;
namespace AgroSolutions.Ingestion.Application.Services;

public interface IIngestionService
{
    Task ProcessSensorDataAsync(SensorDataRequest request);
}