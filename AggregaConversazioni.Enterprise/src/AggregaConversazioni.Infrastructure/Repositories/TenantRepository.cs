using Microsoft.EntityFrameworkCore;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;
using AggregaConversazioni.Infrastructure.Data;

namespace AggregaConversazioni.Infrastructure.Repositories;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Name == name, cancellationToken);
    }
}
