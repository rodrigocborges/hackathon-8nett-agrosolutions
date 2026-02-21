using AgroSolutions.Management.Domain.Entities;
using AgroSolutions.Management.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Management.Infrastructure.Repositories;

public class ProducerRepository : IProducerRepository
{
    private readonly AppDbContext _context;
    public ProducerRepository(AppDbContext context) => _context = context;

    public async Task AddAsync(Producer producer)
    {
        await _context.Producers.AddAsync(producer);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id) => await _context.Producers.AnyAsync(p => p.Id == id);
}