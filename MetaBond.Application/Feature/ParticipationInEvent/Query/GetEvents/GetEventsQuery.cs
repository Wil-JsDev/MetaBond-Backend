using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;

public sealed class GetEventsQuery : IQuery<PagedResult<EventsWithParticipationInEventDTos>>
{
    public Guid? ParticipationInEventId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}