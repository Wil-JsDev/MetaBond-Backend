using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Events.Query.FilterByDateRange;

public sealed class FilterByDateRangeEventsQuery : IQuery<PagedResult<EventsDto>>
{
    public Guid CommunitiesId { get; set; }
    public DateRangeFilter DateRangeFilter { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}