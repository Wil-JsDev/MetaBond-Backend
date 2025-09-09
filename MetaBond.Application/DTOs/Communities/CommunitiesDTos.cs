namespace MetaBond.Application.DTOs.Communities;

public sealed record CommunitiesDTos
(
    Guid CommunitiesId,
    string? Name,
    DateTime CreatedAt,
    Guid CategoryId
);