using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;

public sealed class GetEventsWithParticipationInEventQuery : IQuery<PagedResult<EventsWithParticipationInEventsDTos>>
{
    public Guid? EventsId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}