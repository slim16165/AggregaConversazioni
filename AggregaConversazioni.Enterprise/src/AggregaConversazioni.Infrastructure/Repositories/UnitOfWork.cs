using Microsoft.EntityFrameworkCore.Storage;
using AggregaConversazioni.Domain.Interfaces;
using AggregaConversazioni.Infrastructure.Data;

namespace AggregaConversazioni.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private ITransformationRepository? _transformations;
    private IUserRepository? _users;
    private ITenantRepository? _tenants;
    private ITransformationRuleRepository? _transformationRules;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public ITransformationRepository Transformations
    {
        get
        {
            _transformations ??= new TransformationRepository(_context);
            return _transformations;
        }
    }

    public IUserRepository Users
    {
        get
        {
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    public ITenantRepository Tenants
    {
        get
        {
            _tenants ??= new TenantRepository(_context);
            return _tenants;
        }
    }

    public ITransformationRuleRepository TransformationRules
    {
        get
        {
            _transformationRules ??= new TransformationRuleRepository(_context);
            return _transformationRules;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
