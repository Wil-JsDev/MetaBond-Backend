namespace MetaBond.Application.DTOs.Events;

public record CommunitySummaryDto(
    string? Description,
    string? Category,
    DateTime CreatedAt
    );