namespace AgroSolutions.Alerts.Domain.Entities;

public class Alert
{
    public Guid Id { get; private set; }
    public Guid FieldId { get; private set; }
    public Field Field { get; private set; } // Navegação
    public string Type { get; private set; } // Ex: "Umidade Baixa", "Temperatura Crítica"
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Alert() { }

    public Alert(Guid fieldId, string type, string message)
    {
        Id = Guid.NewGuid();
        FieldId = fieldId;
        Type = type;
        Message = message;
        CreatedAt = DateTime.UtcNow;
    }
}