using Microsoft.EntityFrameworkCore;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;
using AggregaConversazioni.Infrastructure.Data;

namespace AggregaConversazioni.Infrastructure.Repositories;

public class TransformationRepository : Repository<Transformation>, ITransformationRepository
{
    public TransformationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Transformation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transformation>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.TenantId == tenantId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Transformation>> GetByStatusAsync(TransformationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
