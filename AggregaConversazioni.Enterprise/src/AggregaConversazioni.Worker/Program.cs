using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AggregaConversazioni.Application;
using AggregaConversazioni.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

// Configuration
var configuration = builder.Configuration;

// Add services
builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

// Hangfire
var connectionString = configuration.GetConnectionString("PostgreSQL");
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(connectionString));
builder.Services.AddHangfireServer();

var host = builder.Build();

// Configure Hangfire dashboard (opzionale, solo per sviluppo)
if (host.Environment.IsDevelopment())
{
    // Dashboard disponibile su /hangfire
}

host.Run();
