namespace MetaBond.Application.DTOs.Communities;

public sealed record CommunitiesByCategoryDto(
    Guid CommunitiesId,
    string? Name,
    DateTime CreatedAt
);