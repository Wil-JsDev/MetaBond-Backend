using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;

public class GetEventsByTitleAndCommunityIdQuery : IQuery<PagedResult<EventsDto>>
{
    public Guid CommunitiesId { get; set; }

    public string? Title { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}