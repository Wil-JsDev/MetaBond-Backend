namespace MetaBond.Application.DTOs.Events;

public sealed record CommunitySummaryDto
(
    string? Description,
    string? Category,
    DateTime CreatedAt
);