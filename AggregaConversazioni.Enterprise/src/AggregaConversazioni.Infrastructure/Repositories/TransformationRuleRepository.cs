using Microsoft.EntityFrameworkCore;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;
using AggregaConversazioni.Infrastructure.Data;

namespace AggregaConversazioni.Infrastructure.Repositories;

public class TransformationRuleRepository : Repository<TransformationRule>, ITransformationRuleRepository
{
    public TransformationRuleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TransformationRule>> GetBySourceTypeAsync(TransformationType sourceType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SourceType == sourceType)
            .OrderBy(r => r.Order)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TransformationRule>> GetActiveRulesAsync(TransformationType sourceType, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.SourceType == sourceType && r.IsActive)
            .OrderBy(r => r.Order)
            .ToListAsync(cancellationToken);
    }
}
