using AgroSolutions.Alerts.Domain.Entities;

public interface IAlertRepository
{
    Task AddAsync(Alert alert);
    Task<IEnumerable<Alert>> GetByFieldIdAsync(Guid fieldId);
}