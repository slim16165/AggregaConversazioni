namespace AggregaConversazioni.Domain.Entities;

public class Transformation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public TransformationType SourceType { get; set; }
    public string Input { get; set; } = string.Empty;
    public string Output { get; set; } = string.Empty;
    public TransformationStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? ProcessingTime { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}

public enum TransformationType
{
    Messenger,
    Instagram,
    Telegram,
    IoLeiCiclico,
    Evernote,
    Facebook,
    MarkdownToWiki,
    MarkdownToWikiPandoc
}

public enum TransformationStatus
{
    Pending,
    Processing,
    Completed,
    Failed
}
