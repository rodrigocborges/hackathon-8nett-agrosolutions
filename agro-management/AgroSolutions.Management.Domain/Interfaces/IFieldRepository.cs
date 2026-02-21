using AgroSolutions.Management.Domain.Entities;

namespace AgroSolutions.Management.Domain.Interfaces;

public interface IFieldRepository
{
    Task AddAsync(Field field);
    Task<IEnumerable<Field>> GetAllByPropertyIdAsync(Guid propertyId);
}