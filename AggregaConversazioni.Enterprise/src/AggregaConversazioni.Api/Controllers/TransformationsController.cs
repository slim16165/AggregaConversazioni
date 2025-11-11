using Microsoft.AspNetCore.Mvc;
using MediatR;
using AggregaConversazioni.Application.Features.Transformations.Commands;
using AggregaConversazioni.Application.Features.Transformations.Queries;
using AggregaConversazioni.Domain.Entities;

namespace AggregaConversazioni.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TransformationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TransformationsController> _logger;

    public TransformationsController(IMediator mediator, ILogger<TransformationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Trasforma un testo usando il transformer specificato
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransformTextResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransformTextResult>> TransformText([FromBody] TransformTextRequest request)
    {
        // TODO: Ottenere UserId da JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var command = new TransformTextCommand
        {
            UserId = userId,
            TenantId = request.TenantId,
            SourceType = request.SourceType,
            Input = request.Input,
            Options = request.Options
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Ottiene una trasformazione specifica
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Transformation), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Transformation>> GetTransformation(Guid id)
    {
        // TODO: Ottenere UserId da JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var query = new GetTransformationQuery
        {
            TransformationId = id,
            UserId = userId
        };

        var transformation = await _mediator.Send(query);
        
        if (transformation == null)
        {
            return NotFound();
        }

        return Ok(transformation);
    }

    /// <summary>
    /// Ottiene tutte le trasformazioni dell'utente
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Transformation>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Transformation>>> GetUserTransformations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TransformationType? sourceType = null,
        [FromQuery] TransformationStatus? status = null)
    {
        // TODO: Ottenere UserId da JWT token
        var userId = Guid.NewGuid(); // Placeholder

        var query = new GetUserTransformationsQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            SourceType = sourceType,
            Status = status
        };

        var transformations = await _mediator.Send(query);
        return Ok(transformations);
    }
}

public class TransformTextRequest
{
    public TransformationType SourceType { get; set; }
    public string Input { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public Dictionary<string, object>? Options { get; set; }
}
