namespace AgroSolutions.Alerts.Domain.Entities;

public class Field
{
    public Guid Id { get; private set; }
    public string CropType { get; private set; }
    public Guid PropertyId { get; private set; }

    protected Field() { }

    public Field(Guid id, string cropType, Guid propertyId)
    {
        Id = id;
        CropType = cropType;
        PropertyId = propertyId;
    }
}