using MediatR;
using AggregaConversazioni.Application.Transformers;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AggregaConversazioni.Application.Features.Transformations.Commands;

public class TransformTextCommandHandler : IRequestHandler<TransformTextCommand, TransformTextResult>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TransformTextCommandHandler> _logger;

    public TransformTextCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<TransformTextCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<TransformTextResult> Handle(TransformTextCommand request, CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;
        var transformation = new Transformation
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            TenantId = request.TenantId,
            SourceType = request.SourceType,
            Input = request.Input,
            Status = TransformationStatus.Processing,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            // Ottieni il transformer appropriato
            var transformer = TransformerFactory.Create(request.SourceType);
            
            // Se è Markdown e l'opzione usePandoc è true, usa Pandoc
            if (request.SourceType == TransformationType.MarkdownToWiki && 
                request.Options?.ContainsKey("usePandoc") == true &&
                request.Options["usePandoc"] is true)
            {
                transformer = TransformerFactory.Create(TransformationType.MarkdownToWikiPandoc);
            }

            // Esegui la trasformazione
            var output = transformer.Transform(request.Input);
            var processingTime = DateTime.UtcNow - startTime;

            transformation.Output = output;
            transformation.Status = TransformationStatus.Completed;
            transformation.CompletedAt = DateTime.UtcNow;
            transformation.ProcessingTime = processingTime;
            transformation.Metadata = request.Options;

            await _unitOfWork.Transformations.AddAsync(transformation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Transformation completed: {TransformationId}, Type: {SourceType}, ProcessingTime: {ProcessingTime}ms",
                transformation.Id, request.SourceType, processingTime.TotalMilliseconds);

            return new TransformTextResult
            {
                TransformationId = transformation.Id,
                Output = output,
                Status = TransformationStatus.Completed,
                ProcessingTime = processingTime
            };
        }
        catch (Exception ex)
        {
            var processingTime = DateTime.UtcNow - startTime;
            transformation.Status = TransformationStatus.Failed;
            transformation.ErrorMessage = ex.Message;
            transformation.CompletedAt = DateTime.UtcNow;
            transformation.ProcessingTime = processingTime;

            await _unitOfWork.Transformations.AddAsync(transformation, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogError(ex,
                "Transformation failed: {TransformationId}, Type: {SourceType}, Error: {Error}",
                transformation.Id, request.SourceType, ex.Message);

            return new TransformTextResult
            {
                TransformationId = transformation.Id,
                Output = string.Empty,
                Status = TransformationStatus.Failed,
                ProcessingTime = processingTime,
                ErrorMessage = ex.Message
            };
        }
    }
}
