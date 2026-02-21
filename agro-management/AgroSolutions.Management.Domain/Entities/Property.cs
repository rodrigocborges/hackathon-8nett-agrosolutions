namespace AgroSolutions.Management.Domain.Entities;

public class Property
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Guid ProducerId { get; private set; }
    public Producer Producer { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected Property() { }

    public Property(string name, Guid producerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        ProducerId = producerId;
        CreatedAt = DateTime.UtcNow;
    }
}