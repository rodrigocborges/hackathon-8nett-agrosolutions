using AgroSolutions.Management.Domain.Entities;

namespace AgroSolutions.Management.Domain.Interfaces;

public interface IPropertyRepository
{
    Task AddAsync(Property property);
    Task<Property?> GetByIdAsync(Guid id);
    Task<IEnumerable<Property>> GetAllByProducerIdAsync(Guid producerId);
}
