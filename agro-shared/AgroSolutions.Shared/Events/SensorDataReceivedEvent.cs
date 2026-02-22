namespace AgroSolutions.Shared.Events;

public record SensorDataReceivedEvent(Guid FieldId, double SoilMoisture, double Temperature, double Precipitation, DateTime Timestamp);