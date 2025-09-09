namespace MetaBond.Application.DTOs.InterestCategory;

public record UpdateInterestCategoryDTos(
    Guid InterestCategoryId,
    string Name,
    DateTime? UpdateAt
);