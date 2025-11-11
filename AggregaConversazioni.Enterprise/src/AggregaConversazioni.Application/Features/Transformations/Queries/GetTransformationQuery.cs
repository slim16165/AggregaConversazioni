using MediatR;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;

namespace AggregaConversazioni.Application.Features.Transformations.Queries;

public class GetTransformationQuery : IRequest<Transformation?>
{
    public Guid TransformationId { get; set; }
    public Guid? UserId { get; set; } // Per autorizzazione
}

public class GetTransformationQueryHandler : IRequestHandler<GetTransformationQuery, Transformation?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTransformationQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Transformation?> Handle(GetTransformationQuery request, CancellationToken cancellationToken)
    {
        var transformation = await _unitOfWork.Transformations.GetByIdAsync(request.TransformationId, cancellationToken);
        
        // Verifica autorizzazione
        if (transformation != null && request.UserId.HasValue && transformation.UserId != request.UserId.Value)
        {
            return null; // Non autorizzato
        }

        return transformation;
    }
}
