using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitle;

public sealed class FilterByTitleEventsQuery : IQuery<PagedResult<EventsDto>>
{
    public string? Title { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}