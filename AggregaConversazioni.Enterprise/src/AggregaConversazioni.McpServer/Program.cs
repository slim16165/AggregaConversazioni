using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AggregaConversazioni.McpServer.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(configure => configure.AddConsole());
builder.Services.AddSingleton<McpServer>();

var app = builder.Build();

// Endpoint HTTP per MCP
app.MapPost("/mcp", async (HttpContext context, McpServer server) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var requestJson = await reader.ReadToEndAsync();

    var responseJson = await server.HandleRequestAsync(requestJson);

    context.Response.ContentType = "application/json";
    await context.Response.WriteAsync(responseJson);
});

app.Run("http://localhost:5001");
