namespace AgroSolutions.Ingestion.Domain.Entities;

public class SensorReading
{
    public Guid Id { get; private set; }
    public Guid FieldId { get; private set; }
    public double SoilMoisture { get; private set; } // % de Umidade
    public double Temperature { get; private set; }  // Graus Celsius
    public double Precipitation { get; private set; } // mm de chuva
    public DateTime ReceivedAt { get; private set; }

    protected SensorReading() { }

    public SensorReading(Guid fieldId, double soilMoisture, double temperature, double precipitation)
    {
        Id = Guid.NewGuid();
        FieldId = fieldId;
        SoilMoisture = soilMoisture;
        Temperature = temperature;
        Precipitation = precipitation;
        ReceivedAt = DateTime.UtcNow;
    }
}