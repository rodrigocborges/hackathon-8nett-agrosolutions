using AgroSolutions.Identity.Domain.Entities;

namespace AgroSolutions.Identity.Domain.Interfaces;

public interface IProducerRepository
{
    Task<Producer?> GetByEmailAsync(string email);
    Task AddAsync(Producer producer);
}