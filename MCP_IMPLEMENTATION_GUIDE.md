# 🔌 Guida Implementazione MCP (Model Context Protocol)

## Panoramica
Guida tecnica dettagliata per implementare un server MCP che espone le funzionalità di AggregaConversazioni agli AI assistants.

---

## 📚 Cos'è MCP?

**Model Context Protocol (MCP)** è un protocollo standardizzato che permette agli AI assistants di interagire con sistemi esterni attraverso:
- **Tools**: Funzioni che l'AI può chiamare
- **Resources**: Dati che l'AI può leggere
- **Prompts**: Template di prompt riutilizzabili

---

## 🏗️ Architettura MCP Server

```
┌─────────────────┐
│  AI Assistant   │
│  (Claude/GPT)   │
└────────┬────────┘
         │ JSON-RPC 2.0
         │ (WebSocket/HTTP)
         ▼
┌─────────────────┐
│   MCP Server    │
│  (AggregaConv)  │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│ Business Logic  │
│  (Transformers) │
└─────────────────┘
```

---

## 📦 Step 1: Setup Progetto MCP Server

### Creazione Progetto

```bash
dotnet new classlib -n AggregaConversazioni.McpServer -f net8.0
cd AggregaConversazioni.McpServer
dotnet add package System.Text.Json
dotnet add package Microsoft.Extensions.Logging
dotnet add package Microsoft.Extensions.DependencyInjection
```

### Struttura Progetto

```
AggregaConversazioni.McpServer/
├── Models/
│   ├── McpRequest.cs
│   ├── McpResponse.cs
│   └── McpError.cs
├── Tools/
│   ├── TransformConversationTool.cs
│   ├── ListTransformersTool.cs
│   └── ValidateTransformationTool.cs
├── Resources/
│   ├── TransformationTemplatesResource.cs
│   └── RegexRulesResource.cs
├── Prompts/
│   └── ConversationAnalysisPrompt.cs
├── Server/
│   ├── McpServer.cs
│   └── McpMessageHandler.cs
└── Program.cs
```

---

## 🔧 Step 2: Implementazione Modelli MCP

### McpRequest.cs

```csharp
using System.Text.Json.Serialization;

namespace AggregaConversazioni.McpServer.Models;

public class McpRequest
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("method")]
    public string Method { get; set; } = string.Empty;

    [JsonPropertyName("params")]
    public object? Params { get; set; }
}

public class InitializeParams
{
    [JsonPropertyName("protocolVersion")]
    public string ProtocolVersion { get; set; } = "2024-11-05";

    [JsonPropertyName("capabilities")]
    public ClientCapabilities? Capabilities { get; set; }

    [JsonPropertyName("clientInfo")]
    public ClientInfo? ClientInfo { get; set; }
}

public class ClientCapabilities
{
    [JsonPropertyName("tools")]
    public object? Tools { get; set; }
}

public class ClientInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;
}
```

### McpResponse.cs

```csharp
using System.Text.Json.Serialization;

namespace AggregaConversazioni.McpServer.Models;

public class McpResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("id")]
    public object? Id { get; set; }

    [JsonPropertyName("result")]
    public object? Result { get; set; }

    [JsonPropertyName("error")]
    public McpError? Error { get; set; }
}

public class McpError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public object? Data { get; set; }
}
```

---

## 🛠️ Step 3: Implementazione Tools

### TransformConversationTool.cs

```csharp
using System.Text.Json.Serialization;
using AggregaConversazioni.Parsers;
using AggregaConversazioni.Transformers;

namespace AggregaConversazioni.McpServer.Tools;

public class TransformConversationTool
{
    [JsonPropertyName("name")]
    public string Name => "transform_conversation";

    [JsonPropertyName("description")]
    public string Description => 
        "Trasforma conversazioni da social media (Messenger, Telegram, Instagram) " +
        "o note Evernote in formato MediaWiki. Supporta anche conversione Markdown to Wiki.";

    [JsonPropertyName("inputSchema")]
    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Required = new[] { "sourceType", "content" },
        Properties = new Dictionary<string, PropertyDefinition>
        {
            ["sourceType"] = new PropertyDefinition
            {
                Type = "string",
                Enum = new[] { "messenger", "telegram", "instagram", "evernote", "markdown" },
                Description = "Tipo di sorgente da convertire"
            },
            ["content"] = new PropertyDefinition
            {
                Type = "string",
                Description = "Contenuto da trasformare"
            },
            ["options"] = new PropertyDefinition
            {
                Type = "object",
                Description = "Opzioni aggiuntive per la trasformazione",
                Properties = new Dictionary<string, PropertyDefinition>
                {
                    ["usePandoc"] = new PropertyDefinition
                    {
                        Type = "boolean",
                        Description = "Usa Pandoc per conversione Markdown (solo per sourceType=markdown)"
                    },
                    ["preserveFormatting"] = new PropertyDefinition
                    {
                        Type = "boolean",
                        Description = "Mantieni formattazione originale quando possibile"
                    }
                }
            }
        }
    };

    public async Task<ToolResult> ExecuteAsync(
        TransformConversationInput input,
        ITextTransformer transformer)
    {
        try
        {
            var result = transformer.Transform(input.Content);
            
            return new ToolResult
            {
                Content = new[]
                {
                    new ContentItem
                    {
                        Type = "text",
                        Text = result
                    }
                },
                IsError = false
            };
        }
        catch (Exception ex)
        {
            return new ToolResult
            {
                Content = new[]
                {
                    new ContentItem
                    {
                        Type = "text",
                        Text = $"Errore durante la trasformazione: {ex.Message}"
                    }
                },
                IsError = true
            };
        }
    }
}

public class TransformConversationInput
{
    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public TransformationOptions? Options { get; set; }
}

public class TransformationOptions
{
    [JsonPropertyName("usePandoc")]
    public bool UsePandoc { get; set; }

    [JsonPropertyName("preserveFormatting")]
    public bool PreserveFormatting { get; set; }
}

public class ToolResult
{
    [JsonPropertyName("content")]
    public ContentItem[] Content { get; set; } = Array.Empty<ContentItem>();

    [JsonPropertyName("isError")]
    public bool IsError { get; set; }
}

public class ContentItem
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}

public class ToolInputSchema
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public string[]? Required { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertyDefinition>? Properties { get; set; }
}

public class PropertyDefinition
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("enum")]
    public string[]? Enum { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, PropertyDefinition>? Properties { get; set; }
}
```

### ListTransformersTool.cs

```csharp
using System.Text.Json.Serialization;

namespace AggregaConversazioni.McpServer.Tools;

public class ListTransformersTool
{
    [JsonPropertyName("name")]
    public string Name => "list_available_transformers";

    [JsonPropertyName("description")]
    public string Description => 
        "Lista tutti i transformer disponibili con le loro caratteristiche e opzioni supportate.";

    [JsonPropertyName("inputSchema")]
    public ToolInputSchema InputSchema => new()
    {
        Type = "object",
        Required = Array.Empty<string>(),
        Properties = new Dictionary<string, PropertyDefinition>()
    };

    public ToolResult Execute()
    {
        var transformers = new[]
        {
            new TransformerInfo
            {
                Id = "messenger",
                Name = "Facebook Messenger",
                Description = "Converte conversazioni da Facebook Messenger in formato MediaWiki",
                SupportedFeatures = new[] { "speaker_identification", "timestamp_preservation" }
            },
            new TransformerInfo
            {
                Id = "telegram",
                Name = "Telegram",
                Description = "Converte conversazioni Telegram in formato MediaWiki",
                SupportedFeatures = new[] { "speaker_identification", "media_links" }
            },
            new TransformerInfo
            {
                Id = "instagram",
                Name = "Instagram",
                Description = "Converte post e commenti Instagram in formato MediaWiki",
                SupportedFeatures = new[] { "hashtags", "mentions", "media_links" }
            },
            new TransformerInfo
            {
                Id = "evernote",
                Name = "Evernote",
                Description = "Converte note Evernote in formato MediaWiki",
                SupportedFeatures = new[] { "hierarchical_structure", "attachments" }
            },
            new TransformerInfo
            {
                Id = "markdown",
                Name = "Markdown to Wiki",
                Description = "Converte Markdown in sintassi MediaWiki (supporta Pandoc)",
                SupportedFeatures = new[] { "pandoc_engine", "regex_engine" }
            }
        };

        return new ToolResult
        {
            Content = new[]
            {
                new ContentItem
                {
                    Type = "text",
                    Text = System.Text.Json.JsonSerializer.Serialize(transformers, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    })
                }
            },
            IsError = false
        };
    }
}

public class TransformerInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("supportedFeatures")]
    public string[] SupportedFeatures { get; set; } = Array.Empty<string>();
}
```

---

## 📄 Step 4: Implementazione Resources

### TransformationTemplatesResource.cs

```csharp
using System.Text.Json.Serialization;

namespace AggregaConversazioni.McpServer.Resources;

public class TransformationTemplatesResource
{
    [JsonPropertyName("uri")]
    public string Uri => "template://transformation-templates";

    [JsonPropertyName("name")]
    public string Name => "Transformation Templates";

    [JsonPropertyName("description")]
    public string Description => 
        "Template predefiniti per trasformazioni comuni con esempi di input/output";

    [JsonPropertyName("mimeType")]
    public string MimeType => "application/json";

    public async Task<ResourceContent> GetContentAsync()
    {
        var templates = new[]
        {
            new TransformationTemplate
            {
                Id = "messenger-basic",
                SourceType = "messenger",
                Description = "Template base per conversazioni Messenger",
                ExampleInput = "[10:00 AM] John: Ciao, come stai?\n[10:01 AM] Jane: Bene, grazie!",
                ExampleOutput = "{{Conversazione|speaker1=John|speaker2=Jane}}\n* '''John:''' Ciao, come stai?\n* '''Jane:''' Bene, grazie!"
            },
            new TransformationTemplate
            {
                Id = "telegram-group",
                SourceType = "telegram",
                Description = "Template per chat di gruppo Telegram",
                ExampleInput = "[12:30] Alice: Hai visto il nuovo film?\n[12:31] Bob: Sì, è fantastico!",
                ExampleOutput = "{{Conversazione|gruppo=Chat Gruppo}}\n* '''Alice:''' Hai visto il nuovo film?\n* '''Bob:''' Sì, è fantastico!"
            }
        };

        return new ResourceContent
        {
            Uri = Uri,
            MimeType = MimeType,
            Text = System.Text.Json.JsonSerializer.Serialize(templates, new System.Text.Json.JsonSerializerOptions 
            { 
                WriteIndented = true 
            })
        };
    }
}

public class TransformationTemplate
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("sourceType")]
    public string SourceType { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("exampleInput")]
    public string ExampleInput { get; set; } = string.Empty;

    [JsonPropertyName("exampleOutput")]
    public string ExampleOutput { get; set; } = string.Empty;
}

public class ResourceContent
{
    [JsonPropertyName("uri")]
    public string Uri { get; set; } = string.Empty;

    [JsonPropertyName("mimeType")]
    public string MimeType { get; set; } = string.Empty;

    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
```

---

## 🖥️ Step 5: Implementazione MCP Server

### McpServer.cs

```csharp
using System.Text.Json;
using AggregaConversazioni.McpServer.Models;
using AggregaConversazioni.McpServer.Tools;
using AggregaConversazioni.McpServer.Resources;
using AggregaConversazioni.Parsers;
using AggregaConversazioni.Transformers;
using Microsoft.Extensions.Logging;

namespace AggregaConversazioni.McpServer.Server;

public class McpServer
{
    private readonly ILogger<McpServer> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<string, ITool> _tools;
    private readonly Dictionary<string, IResource> _resources;

    public McpServer(ILogger<McpServer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _tools = new Dictionary<string, ITool>();
        _resources = new Dictionary<string, IResource>();

        RegisterTools();
        RegisterResources();
    }

    private void RegisterTools()
    {
        _tools["transform_conversation"] = new TransformConversationTool();
        _tools["list_available_transformers"] = new ListTransformersTool();
    }

    private void RegisterResources()
    {
        _resources["template://transformation-templates"] = new TransformationTemplatesResource();
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
                "resources/list" => await HandleResourcesListAsync(request),
                "resources/read" => await HandleResourceReadAsync(request),
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
        var initParams = JsonSerializer.Deserialize<InitializeParams>(
            JsonSerializer.Serialize(request.Params));

        return new McpResponse
        {
            Id = request.Id,
            Result = new
            {
                protocolVersion = "2024-11-05",
                capabilities = new
                {
                    tools = new { },
                    resources = new { }
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
        var toolCall = JsonSerializer.Deserialize<ToolCallParams>(
            JsonSerializer.Serialize(request.Params));

        if (toolCall == null || !_tools.TryGetValue(toolCall.Name, out var tool))
        {
            return CreateErrorResponse(request.Id, -32602, $"Tool not found: {toolCall?.Name}");
        }

        // Esegui il tool
        var result = await ExecuteToolAsync(tool, toolCall.Arguments);

        return new McpResponse
        {
            Id = request.Id,
            Result = result
        };
    }

    private async Task<object> ExecuteToolAsync(ITool tool, JsonElement arguments)
    {
        if (tool is TransformConversationTool transformTool)
        {
            var input = JsonSerializer.Deserialize<TransformConversationInput>(
                arguments.GetRawText());

            if (input == null)
            {
                throw new ArgumentException("Invalid input for transform_conversation");
            }

            // Ottieni il transformer appropriato
            var transformerType = input.SourceType switch
            {
                "messenger" => TransformerType.Messenger,
                "telegram" => TransformerType.Telegram,
                "instagram" => TransformerType.Instagram,
                "evernote" => TransformerType.Evernote,
                "markdown" => input.Options?.UsePandoc == true 
                    ? TransformerType.MarkdownToWikiPandoc 
                    : TransformerType.MarkdownToWiki,
                _ => throw new ArgumentException($"Unknown source type: {input.SourceType}")
            };

            var transformer = TransformerFactory.Create(transformerType);
            var toolResult = await transformTool.ExecuteAsync(input, transformer);

            return new
            {
                content = toolResult.Content.Select(c => new
                {
                    type = c.Type,
                    text = c.Text
                }),
                isError = toolResult.IsError
            };
        }
        else if (tool is ListTransformersTool listTool)
        {
            var result = listTool.Execute();
            return new
            {
                content = result.Content.Select(c => new
                {
                    type = c.Type,
                    text = c.Text
                }),
                isError = result.IsError
            };
        }

        throw new NotImplementedException($"Tool {tool.Name} not implemented");
    }

    private async Task<McpResponse> HandleResourcesListAsync(McpRequest request)
    {
        var resources = _resources.Values.Select(r => new
        {
            uri = r.Uri,
            name = r.Name,
            description = r.Description,
            mimeType = r.MimeType
        }).ToArray();

        return new McpResponse
        {
            Id = request.Id,
            Result = new { resources }
        };
    }

    private async Task<McpResponse> HandleResourceReadAsync(McpRequest request)
    {
        var readParams = JsonSerializer.Deserialize<ResourceReadParams>(
            JsonSerializer.Serialize(request.Params));

        if (readParams == null || !_resources.TryGetValue(readParams.Uri, out var resource))
        {
            return CreateErrorResponse(request.Id, -32602, $"Resource not found: {readParams?.Uri}");
        }

        var content = await resource.GetContentAsync();

        return new McpResponse
        {
            Id = request.Id,
            Result = new
            {
                contents = new[]
                {
                    new
                    {
                        uri = content.Uri,
                        mimeType = content.MimeType,
                        text = content.Text
                    }
                }
            }
        };
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

public interface IResource
{
    string Uri { get; }
    string Name { get; }
    string Description { get; }
    string MimeType { get; }
    Task<ResourceContent> GetContentAsync();
}

public class ToolCallParams
{
    public string Name { get; set; } = string.Empty;
    public JsonElement Arguments { get; set; }
}

public class ResourceReadParams
{
    public string Uri { get; set; } = string.Empty;
}
```

---

## 🚀 Step 6: Setup HTTP/WebSocket Server

### Program.cs

```csharp
using AggregaConversazioni.McpServer.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configurazione servizi
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

// WebSocket endpoint (opzionale, per comunicazione bidirezionale)
app.Map("/mcp-ws", async (HttpContext context, McpServer server) =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
    var buffer = new byte[1024 * 4];

    while (webSocket.State == System.Net.WebSockets.WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(
            new ArraySegment<byte>(buffer), 
            CancellationToken.None);

        if (result.MessageType == System.Net.WebSockets.WebSocketMessageType.Text)
        {
            var requestJson = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
            var responseJson = await server.HandleRequestAsync(requestJson);
            
            var responseBytes = System.Text.Encoding.UTF8.GetBytes(responseJson);
            await webSocket.SendAsync(
                new ArraySegment<byte>(responseBytes),
                System.Net.WebSockets.WebSocketMessageType.Text,
                true,
                CancellationToken.None);
        }
    }
});

app.Run("http://localhost:5000");
```

---

## 🧪 Step 7: Testing MCP Server

### Test con cURL

```bash
# Initialize
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 1,
    "method": "initialize",
    "params": {
      "protocolVersion": "2024-11-05",
      "capabilities": {},
      "clientInfo": {
        "name": "test-client",
        "version": "1.0.0"
      }
    }
  }'

# List Tools
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 2,
    "method": "tools/list"
  }'

# Call Tool
curl -X POST http://localhost:5000/mcp \
  -H "Content-Type: application/json" \
  -d '{
    "jsonrpc": "2.0",
    "id": 3,
    "method": "tools/call",
    "params": {
      "name": "transform_conversation",
      "arguments": {
        "sourceType": "messenger",
        "content": "[10:00 AM] John: Ciao, come stai?"
      }
    }
  }'
```

---

## 📝 Step 8: Configurazione per AI Assistants

### Configurazione Claude Desktop

Crea `~/Library/Application Support/Claude/claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "aggrega-conversazioni": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/path/to/AggregaConversazioni.McpServer"
      ],
      "env": {
        "ASPNETCORE_URLS": "http://localhost:5000"
      }
    }
  }
}
```

### Configurazione Cursor/VS Code

Per integrare MCP in Cursor, aggiungi alla configurazione:

```json
{
  "mcp": {
    "servers": {
      "aggrega-conversazioni": {
        "url": "http://localhost:5000/mcp",
        "transport": "http"
      }
    }
  }
}
```

---

## 🎯 Esempi di Utilizzo

### Esempio 1: Conversione Messenger

```json
{
  "method": "tools/call",
  "params": {
    "name": "transform_conversation",
    "arguments": {
      "sourceType": "messenger",
      "content": "[10:00 AM] John: Ciao!\n[10:01 AM] Jane: Ciao John!"
    }
  }
}
```

### Esempio 2: Conversione Markdown con Pandoc

```json
{
  "method": "tools/call",
  "params": {
    "name": "transform_conversation",
    "arguments": {
      "sourceType": "markdown",
      "content": "# Titolo\n**Testo in grassetto**",
      "options": {
        "usePandoc": true
      }
    }
  }
}
```

---

## 🔒 Step 9: Sicurezza e Autenticazione

### Aggiungere Autenticazione API Key

```csharp
// Aggiungi middleware per autenticazione
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.TryGetValue("X-API-Key", out var apiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("API Key required");
        return;
    }

    // Valida API key (da database o configurazione)
    if (!IsValidApiKey(apiKey))
    {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid API Key");
        return;
    }

    await next();
});
```

---

## 📊 Step 10: Monitoring e Logging

### Aggiungere Telemetria

```csharp
builder.Services.AddApplicationInsightsTelemetry();

// Logging strutturato
builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
    configure.AddApplicationInsights();
    configure.AddSeq("http://localhost:5341"); // Se usi Seq
});
```

---

## ✅ Checklist Implementazione

- [ ] Progetto MCP Server creato
- [ ] Modelli MCP implementati
- [ ] Tools implementati e testati
- [ ] Resources implementate
- [ ] Server HTTP/WebSocket configurato
- [ ] Test di integrazione completati
- [ ] Documentazione API completata
- [ ] Sicurezza implementata
- [ ] Monitoring configurato
- [ ] Deploy su produzione

---

**Versione**: 1.0  
**Ultimo aggiornamento**: 2024
