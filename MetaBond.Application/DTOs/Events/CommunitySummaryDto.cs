namespace MetaBond.Application.DTOs.Events;

public sealed record CommunitySummaryDto(
    string? Description,
    DateTime CreatedAt
);