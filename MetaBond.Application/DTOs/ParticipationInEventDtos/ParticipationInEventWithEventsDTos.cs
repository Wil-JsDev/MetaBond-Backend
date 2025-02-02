
namespace MetaBond.Application.DTOs.ParticipationInEventDtos
{
    public sealed record ParticipationInEventWithEventsDTos
    (
        Guid? ParticipationInEventId,
        Guid? EventId,
        ICollection<Domain.Models.Events> Events
    );
}
