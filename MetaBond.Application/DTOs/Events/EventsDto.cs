
namespace MetaBond.Application.DTOs.Events;

public sealed record EventsDto
(
    Guid? Id,
    string? Description,
    string? Title,
    DateTime? DateAndTime,
    DateTime? CreatedAt,
    Guid? CommunitiesId
);