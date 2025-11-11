using System.Text.Json;
using AggregaConversazioni.McpServer.Models;
using AggregaConversazioni.McpServer.Tools;
using Microsoft.Extensions.Logging;

namespace AggregaConversazioni.McpServer.Server;

public class McpServer
{
    private readonly ILogger<McpServer> _logger;
    private readonly Dictionary<string, ITool> _tools;

    public McpServer(ILogger<McpServer> logger)
    {
        _logger = logger;
        _tools = new Dictionary<string, ITool>();

        RegisterTools();
    }

    private void RegisterTools()
    {
        _tools["transform_conversation"] = new TransformConversationTool();
    }

    public async Task<string> HandleRequestAsync(string requestJson)
    {
        try
        {
            var request = JsonSerializer.Deserialize<McpRequest>(requestJson);
            if (request == null)
            {
                return CreateErrorResponse(null, -32700, "Parse error");
            }

            var response = request.Method switch
            {
                "initialize" => await HandleInitializeAsync(request),
                "tools/list" => await HandleToolsListAsync(request),
                "tools/call" => await HandleToolCallAsync(request),
                _ => CreateErrorResponse(request.Id, -32601, $"Method not found: {request.Method}")
            };

            return JsonSerializer.Serialize(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling MCP request");
            return CreateErrorResponse(null, -32603, $"Internal error: {ex.Message}");
        }
    }

    private async Task<McpResponse> HandleInitializeAsync(McpRequest request)
    {
        return new McpResponse
        {
            Id = request.Id,
            Result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { }
                },
                serverInfo = new
                {
                    name = "AggregaConversazioni",
                    version = "2.0.0"
                }
            }
        };
    }

    private async Task<McpResponse> HandleToolsListAsync(McpRequest request)
    {
        var tools = _tools.Values.Select(t => new
        {
            name = t.Name,
            description = t.Description,
            inputSchema = t.InputSchema
        }).ToArray();

        return new McpResponse
        {
            Id = request.Id,
            Result = new { tools }
        };
    }

    private async Task<McpResponse> HandleToolCallAsync(McpRequest request)
    {
        var toolCallJson = JsonSerializer.Serialize(request.Params);
        var toolCall = JsonSerializer.Deserialize<JsonElement>(toolCallJson);

        if (!toolCall.TryGetProperty("name", out var nameElement))
        {
            return CreateErrorResponse(request.Id, -32602, "Tool name not provided");
        }

        var toolName = nameElement.GetString();
        if (toolName == null || !_tools.TryGetValue(toolName, out var tool))
        {
            return CreateErrorResponse(request.Id, -32602, $"Tool not found: {toolName}");
        }

        if (tool is TransformConversationTool transformTool)
        {
            var argumentsJson = toolCall.GetProperty("arguments").GetRawText();
            var input = JsonSerializer.Deserialize<TransformConversationInput>(argumentsJson);

            if (input == null)
            {
                return CreateErrorResponse(request.Id, -32602, "Invalid arguments");
            }

            var result = await transformTool.ExecuteAsync(input);

            return new McpResponse
            {
                Id = request.Id,
                Result = new
                {
                    content = result.Content.Select(c => new
                    {
                        type = c.Type,
                        text = c.Text
                    }),
                    isError = result.IsError
                }
            };
        }

        return CreateErrorResponse(request.Id, -32602, $"Tool {toolName} not implemented");
    }

    private McpResponse CreateErrorResponse(object? id, int code, string message)
    {
        return new McpResponse
        {
            Id = id,
            Error = new McpError
            {
                Code = code,
                Message = message
            }
        };
    }
}

public interface ITool
{
    string Name { get; }
    string Description { get; }
    ToolInputSchema InputSchema { get; }
}
