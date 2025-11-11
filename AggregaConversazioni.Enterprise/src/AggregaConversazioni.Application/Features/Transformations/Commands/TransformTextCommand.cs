using MediatR;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Application.Features.Transformations.Commands;

public class TransformTextCommand : IRequest<TransformTextResult>
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public TransformationType SourceType { get; set; }
    public string Input { get; set; } = string.Empty;
    public Dictionary<string, object>? Options { get; set; }
}

public class TransformTextResult
{
    public Guid TransformationId { get; set; }
    public string Output { get; set; } = string.Empty;
    public TransformationStatus Status { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public string? ErrorMessage { get; set; }
}
