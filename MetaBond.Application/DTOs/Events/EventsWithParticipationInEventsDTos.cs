namespace MetaBond.Application.DTOs.Events;

public sealed record EventsWithParticipationInEventsDTos(
    Guid? EventsId,
    string? Description,
    string? Title,
    DateTime? DateAndTime,
    IEnumerable<ParticipationInEventBasicDTos> ParticipationInEvents,
    DateTime? CreatedAt
);