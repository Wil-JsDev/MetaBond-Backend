using MetaBond.Domain.Models;

namespace MetaBond.Application.DTOs.Events
{
    public sealed record EventsWithParticipationInEventsDTos
    (
        Guid? EventsId,
        string? Description,
        string? Title,
        DateTime? DateAndTime,
        ICollection<EventParticipation>? EventParticipations,
        DateTime? CreatedAt
    );
}
