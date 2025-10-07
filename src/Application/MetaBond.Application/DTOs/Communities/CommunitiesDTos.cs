namespace MetaBond.Application.DTOs.Communities;

public sealed record CommunitiesDTos(
    Guid CommunitiesId,
    string? Name,
    DateTime CreatedAt,
    string? Description,
    string? Image,
    Guid CategoryId
);