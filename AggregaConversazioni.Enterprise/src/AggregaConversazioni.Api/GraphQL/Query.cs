using HotChocolate;
using HotChocolate.Types;
using MediatR;
using AggregaConversazioni.Application.Features.Transformations.Queries;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Api.GraphQL;

[ExtendObjectType("Query")]
public class TransformationQuery
{
    public async Task<Transformation?> GetTransformation(
        [Service] IMediator mediator,
        Guid id)
    {
        var query = new GetTransformationQuery
        {
            TransformationId = id
        };
        return await mediator.Send(query);
    }

    public async Task<IEnumerable<Transformation>> GetUserTransformations(
        [Service] IMediator mediator,
        Guid userId,
        int page = 1,
        int pageSize = 20,
        TransformationType? sourceType = null,
        TransformationStatus? status = null)
    {
        var query = new GetUserTransformationsQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            SourceType = sourceType,
            Status = status
        };
        return await mediator.Send(query);
    }
}

public class Query
{
    // Root query type per HotChocolate
}
