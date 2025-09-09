namespace MetaBond.Application.DTOs.InterestCategory;

public sealed record InterestCategoryDTos
(
    Guid InterestCategoryId,
    string Name,
    DateTime CreatedAt
);