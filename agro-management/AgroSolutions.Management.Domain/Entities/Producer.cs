namespace AgroSolutions.Management.Domain.Entities;

public class Producer
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }

    protected Producer() { }

    public Producer(Guid id, string name, string email)
    {
        Id = id;
        Name = name;
        Email = email;
    }
}