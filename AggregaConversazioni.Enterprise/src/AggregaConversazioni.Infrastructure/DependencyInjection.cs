using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AggregaConversazioni.Domain.Interfaces;
using AggregaConversazioni.Infrastructure.Data;
using AggregaConversazioni.Infrastructure.Repositories;
using Serilog;

namespace AggregaConversazioni.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Repository
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ITransformationRepository, TransformationRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<ITransformationRuleRepository, TransformationRuleRepository>();

        // Logging con Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.AddSerilog();
        });

        return services;
    }
}
