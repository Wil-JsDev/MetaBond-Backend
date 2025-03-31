using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;

public sealed class GetEventsQuery : IQuery<IEnumerable<EventsWithParticipationInEventDTos>>
{
    public Guid? ParticipationInEventId { get; set; }
}