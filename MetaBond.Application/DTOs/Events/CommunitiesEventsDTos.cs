namespace MetaBond.Application.DTOs.Events;

public sealed record CommunitiesEventsDTos(
    Guid? Id,
    string? Description,
    string? Title,
    DateTime? DateAndTime,
    DateTime? CreatedAt,
    List<CommunitySummaryDto> Communities
);