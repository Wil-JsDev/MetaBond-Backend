using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Create;
using MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;
using MetaBond.Application.Feature.ParticipationInEvent.Query.GetById;
using MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;
using MetaBond.Application.Feature.ParticipationInEvent.Query.Pagination;
using MetaBond.Application.Helpers;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/participation-in-event")]
public class ParticipationInEventController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new participation in an event",
        Description = "Creates a new participation record in an event using the provided command data."
    )]
    public async Task<ResultT<ParticipationInEventDTos>> CreateAsync(
        [FromBody] CreateParticipationInEventCommand createParticipation,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(createParticipation, cancellationToken);
    }

    [HttpPut]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update participation in an event",
        Description = "Updates an existing participation record using the provided command data."
    )]
    public async Task<ResultT<ParticipationInEventDTos>> UpdateAsync(
        [FromBody] UpdateParticipationInEventCommand updateParticipationInEventCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateParticipationInEventCommand, cancellationToken);
    }

    [HttpGet("{id}")]
    [DisableRateLimiting]
    [SwaggerOperation(
        Summary = "Get participation by ID",
        Description = "Retrieves a specific participation record by its unique identifier."
    )]
    public async Task<ResultT<ParticipationInEventDTos>> GetByIdAsync([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdParticipationInEventQuery { ParticipationInEventId = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{participationInEventId}/events")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get events for a participation",
        Description = "Retrieves the details of events associated with a specific participation."
    )]
    public async Task<ResultT<PagedResult<EventsWithParticipationInEventDTos>>> GetParticipationInEventDetailsAsync(
        [FromRoute] Guid participationInEventId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetEventsQuery
        {
            ParticipationInEventId = participationInEventId,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated participation records",
        Description =
            "Retrieves a paginated list of participation records based on the specified page number and page size."
    )]
    public async Task<ResultT<PagedResult<ParticipationInEventDTos>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedParticipationInEventQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }
}