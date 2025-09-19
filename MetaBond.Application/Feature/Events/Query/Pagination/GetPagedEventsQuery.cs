using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.Pagination;

public sealed class GetPagedEventsQuery : IQuery<PagedResult<EventsDto>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}