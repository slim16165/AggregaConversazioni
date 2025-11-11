namespace AggregaConversazioni.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    ITransformationRepository Transformations { get; }
    IUserRepository Users { get; }
    ITenantRepository Tenants { get; }
    ITransformationRuleRepository TransformationRules { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
