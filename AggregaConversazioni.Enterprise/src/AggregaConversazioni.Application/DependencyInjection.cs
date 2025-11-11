using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AggregaConversazioni.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Registra MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Registra AutoMapper (se necessario)
        // services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
