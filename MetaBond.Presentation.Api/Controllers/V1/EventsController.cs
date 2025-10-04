using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Feature.Events.Commands.Create;
using MetaBond.Application.Feature.Events.Commands.Delete;
using MetaBond.Application.Feature.Events.Commands.Update;
using MetaBond.Application.Feature.Events.Query.FilterByDateRange;
using MetaBond.Application.Feature.Events.Query.FilterByTitle;
using MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;
using MetaBond.Application.Feature.Events.Query.GetById;
using MetaBond.Application.Feature.Events.Query.GetCommunities;
using MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;
using MetaBond.Application.Feature.Events.Query.GetOrderById;
using MetaBond.Application.Feature.Events.Query.Pagination;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using MetaBond.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/events")]
public class EventsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = $"{CommunityMembershipRoleNames.Owner},{CommunityMembershipRoleNames.Moderator}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Create a new event",
        Description = "Creates a new event using the provided command data."
    )]
    public async Task<ResultT<EventsDto>> CreateAsync([FromBody] CreateEventsCommand eventsCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(eventsCommand, cancellationToken);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{CommunityMembershipRoleNames.Owner},{CommunityMembershipRoleNames.Moderator}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Delete an event by ID",
        Description = "Deletes the event identified by the given ID."
    )]
    public async Task<ResultT<Guid>> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new DeleteEventsCommand { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpPut]
    [Authorize(Roles =
        $"{CommunityMembershipRoleNames.Owner},{CommunityMembershipRoleNames.Moderator}, {CommunityMembershipRoleNames.CommunityManager}")]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Update an event",
        Description = "Updates an existing event using the provided command data."
    )]
    public async Task<ResultT<EventsDto>> UpdateAsync([FromBody] UpdateEventsCommand updateEventsCommand,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(updateEventsCommand, cancellationToken);
    }

    [HttpGet("{id}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get event by ID",
        Description = "Retrieves a specific event by its unique identifier."
    )]
    public async Task<ResultT<EventsDto>> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetByIdEventsQuery { Id = id };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("communities/{communitiesId}/search/by-date-range")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter events by date range for a community",
        Description = "Retrieves events for a specific community that fall within the specified date range."
    )]
    public async Task<ResultT<PagedResult<EventsDto>>> FilterByRangeAsync(
        [FromRoute] Guid communitiesId,
        [FromQuery] DateRangeFilter dateRange,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new FilterByDateRangeEventsQuery
        {
            CommunitiesId = communitiesId,
            DateRangeFilter = dateRange,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("search/title/{title}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Filter events by title",
        Description = "Retrieves all events matching the specified title."
    )]
    public async Task<ResultT<PagedResult<EventsDto>>> FilterByTitleAsync([FromRoute] string title,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new FilterByTitleEventsQuery
        {
            Title = title,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("communities/{communityId}/search/title/{title}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get events by title and community",
        Description = "Retrieves events with the specified title within a specific community."
    )]
    public async Task<ResultT<PagedResult<EventsDto>>> GetEventsByTitleAndCommunityIdAsync(
        [FromRoute] Guid communityId,
        [FromRoute] string title,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetEventsByTitleAndCommunityIdQuery
        {
            CommunitiesId = communityId,
            Title = title,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{eventId}/communities")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get event with communities",
        Description = "Retrieves the details of an event including the communities it belongs to."
    )]
    public async Task<ResultT<PagedResult<CommunitiesEventsDTos>>> GetEventsWithCommunitiesAsync(
        [FromRoute] Guid eventId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetEventsDetailsQuery
        {
            Id = eventId,
            PageSize = pageNumber,
            PageNumber = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{eventId}/participation-in-event")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get event participation",
        Description = "Retrieves participation details for a specific event."
    )]
    public async Task<ResultT<PagedResult<EventsWithParticipationInEventsDTos>>> GetEventsWithParticipationInEventAsync(
        [FromRoute] Guid eventId,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetEventsWithParticipationInEventQuery
        {
            EventsId = eventId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("order")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Order events",
        Description = "Retrieves events ordered by the specified field."
    )]
    public async Task<ResultT<PagedResult<EventsDto>>> OrderByIdAsync([FromQuery] string orderBy,
        [FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetOrderByIdEventsQuery
        {
            Order = orderBy,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("pagination")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get paginated events",
        Description = "Retrieves a paginated list of events based on the specified page number and size."
    )]
    public async Task<ResultT<PagedResult<EventsDto>>> GetPagedAsync([FromQuery] int pageNumber,
        [FromQuery] int pageSize,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedEventsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

        return await mediator.Send(query, cancellationToken);
    }
}