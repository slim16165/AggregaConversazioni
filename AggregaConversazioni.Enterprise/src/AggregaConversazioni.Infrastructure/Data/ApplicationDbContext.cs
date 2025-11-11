using Microsoft.EntityFrameworkCore;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Infrastructure.Data.Configurations;

namespace AggregaConversazioni.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Transformation> Transformations => Set<Transformation>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TransformationRule> TransformationRules => Set<TransformationRule>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new TransformationConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new TenantConfiguration());
        modelBuilder.ApplyConfiguration(new TransformationRuleConfiguration());
    }
}
