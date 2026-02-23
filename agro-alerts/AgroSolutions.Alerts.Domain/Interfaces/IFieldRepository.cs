using AgroSolutions.Alerts.Domain.Entities;

namespace AgroSolutions.Alerts.Domain.Interfaces;

public interface IFieldRepository
{
    Task AddAsync(Field field);
    Task<Field?> GetByIdAsync(Guid id);
}