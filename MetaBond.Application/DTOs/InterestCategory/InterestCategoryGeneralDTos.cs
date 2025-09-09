namespace MetaBond.Application.DTOs.InterestCategory;

public sealed record InterestCategoryGeneralDTos
(
    Guid InterestCategoryId,
    string Name,
    DateTime CreatedAt,
    DateTime? UpdateAt
);