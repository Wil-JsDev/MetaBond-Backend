using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.DTOs.ParticipationInEventDtos;

public sealed record EventsWithParticipationInEventDTos
(
    Guid? ParticipationInEventId,
    IEnumerable<EventsDto> Events
);