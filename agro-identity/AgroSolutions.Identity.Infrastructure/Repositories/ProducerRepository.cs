using AgroSolutions.Identity.Domain.Entities;
using AgroSolutions.Identity.Domain.Interfaces;
using AgroSolutions.Identity.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace AgroSolutions.Identity.Infrastructure.Repositories;

public class ProducerRepository : IProducerRepository
{
    private readonly AppDbContext _context;

    public ProducerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Producer producer)
    {
        await _context.Producers.AddAsync(producer);
        await _context.SaveChangesAsync();
    }

    public async Task<Producer?> GetByEmailAsync(string email) => await _context.Producers.FirstOrDefaultAsync(p => p.Email == email);
}