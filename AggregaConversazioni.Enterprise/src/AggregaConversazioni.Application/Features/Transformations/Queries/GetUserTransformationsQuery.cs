using MediatR;
using AggregaConversazioni.Domain.Entities;
using AggregaConversazioni.Domain.Interfaces;

namespace AggregaConversazioni.Application.Features.Transformations.Queries;

public class GetUserTransformationsQuery : IRequest<IEnumerable<Transformation>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public TransformationType? SourceType { get; set; }
    public TransformationStatus? Status { get; set; }
}

public class GetUserTransformationsQueryHandler : IRequestHandler<GetUserTransformationsQuery, IEnumerable<Transformation>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserTransformationsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Transformation>> Handle(GetUserTransformationsQuery request, CancellationToken cancellationToken)
    {
        var transformations = await _unitOfWork.Transformations.GetByUserIdAsync(request.UserId, cancellationToken);

        if (request.SourceType.HasValue)
        {
            transformations = transformations.Where(t => t.SourceType == request.SourceType.Value);
        }

        if (request.Status.HasValue)
        {
            transformations = transformations.Where(t => t.Status == request.Status.Value);
        }

        return transformations
            .OrderByDescending(t => t.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
    }
}
