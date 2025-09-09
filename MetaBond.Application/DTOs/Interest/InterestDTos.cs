namespace MetaBond.Application.DTOs.Interest;

public sealed record InterestDTos
(
    Guid? InterestId,
    string? Name,
    Guid? InterestCategoryId
);