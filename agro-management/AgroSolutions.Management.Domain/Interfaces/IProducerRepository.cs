using AgroSolutions.Management.Domain.Entities;

public interface IProducerRepository
{
    Task AddAsync(Producer producer);
    Task<bool> ExistsAsync(Guid id);
}