using System.Collections;
using MetaBond.Application.DTOs.Events;
using MetaBond.Domain.Models;

namespace MetaBond.Application.DTOs.ParticipationInEventDtos
{
    public sealed record EventsWithParticipationInEventDTos
    (
        Guid? ParticipationInEventId,
        IEnumerable<EventsDto> Events
    );
}
