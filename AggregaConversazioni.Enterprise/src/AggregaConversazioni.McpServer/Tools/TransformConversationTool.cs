using System.Text.Json;
using System.Text.Json.Serialization;
using AggregaConversazioni.Application.Transformers;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.McpServer.Server;

namespace AggregaConversazioni.McpServer.Tools;

public class TransformConversationTool : ITool
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
                    }
                }
            }
        }
    };

    public async Task<ToolResult> ExecuteAsync(TransformConversationInput input)
    {
        try
        {
            var sourceType = input.SourceType.ToLower() switch
            {
                "messenger" => TransformationType.Messenger,
                "telegram" => TransformationType.Telegram,
                "instagram" => TransformationType.Instagram,
                "evernote" => TransformationType.Evernote,
                "markdown" => input.Options?.UsePandoc == true
                    ? TransformationType.MarkdownToWikiPandoc
                    : TransformationType.MarkdownToWiki,
                _ => throw new ArgumentException($"Unknown source type: {input.SourceType}")
            };

            var transformer = TransformerFactory.Create(sourceType);
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
