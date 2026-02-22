namespace AgroSolutions.Ingestion.Application.DTOs;

public record SensorDataRequest(Guid FieldId, double SoilMoisture, double Temperature, double Precipitation);