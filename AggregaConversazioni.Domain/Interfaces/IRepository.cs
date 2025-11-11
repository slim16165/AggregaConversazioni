using System.Linq.Expressions;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
}

public interface ITransformationRepository : IRepository<Transformation>
{
    Task<IEnumerable<Transformation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transformation>> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transformation>> GetByStatusAsync(TransformationStatus status, CancellationToken cancellationToken = default);
}

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

public interface ITenantRepository : IRepository<Tenant>
{
    Task<Tenant?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

public interface ITransformationRuleRepository : IRepository<TransformationRule>
{
    Task<IEnumerable<TransformationRule>> GetBySourceTypeAsync(TransformationType sourceType, CancellationToken cancellationToken = default);
    Task<IEnumerable<TransformationRule>> GetActiveRulesAsync(TransformationType sourceType, CancellationToken cancellationToken = default);
}
