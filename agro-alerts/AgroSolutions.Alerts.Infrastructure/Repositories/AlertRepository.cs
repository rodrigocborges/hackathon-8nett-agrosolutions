using AgroSolutions.Alerts.Domain.Entities;
using AgroSolutions.Alerts.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Alerts.Infrastructure.Repositories;
public class AlertRepository : IAlertRepository
{
    private readonly AppDbContext _context;
    public AlertRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Alert alert)
    {
        await _context.Alerts.AddAsync(alert);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Alert>> GetByFieldIdAsync(Guid fieldId)
        => await _context.Alerts.Where(a => a.FieldId == fieldId).OrderByDescending(a => a.CreatedAt).ToListAsync();
}