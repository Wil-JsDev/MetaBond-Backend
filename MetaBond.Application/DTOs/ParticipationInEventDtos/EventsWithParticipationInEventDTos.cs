using MetaBond.Domain.Models;

namespace MetaBond.Application.DTOs.ParticipationInEventDtos
{
    public sealed record EventsWithParticipationInEventDTos
    (
        Guid? ParticipationInEventId,
        ICollection<EventParticipation>? EventParticipation
    );
}
