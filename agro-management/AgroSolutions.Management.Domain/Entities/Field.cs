namespace AgroSolutions.Management.Domain.Entities;

public class Field
{
    public Guid Id { get; private set; }
    public string CropType { get; private set; } // Cultura (ex: Soja, Milho)
    public double AreaInHectares { get; private set; }
    public Guid PropertyId { get; private set; }
    public Property Property { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Field() { }

    public Field(string cropType, double areaInHectares, Guid propertyId)
    {
        Id = Guid.NewGuid();
        CropType = cropType;
        AreaInHectares = areaInHectares;
        PropertyId = propertyId;
        CreatedAt = DateTime.UtcNow;
    }
}