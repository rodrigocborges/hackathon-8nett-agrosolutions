namespace AgroSolutions.Shared.Events;

public record SensorDataReceivedEvent(Guid FieldId, decimal SoilMoisture, decimal Temperature, DateTime Timestamp);