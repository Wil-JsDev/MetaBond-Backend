using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class InterestCategoryMapper
{
    public static InterestCategoryDTos MapInterestCategoryDTos(InterestCategory interestCategory)
    {
        return new InterestCategoryDTos
        (
            InterestCategoryId: interestCategory.Id,
            Name: interestCategory.Name!,
            CreatedAt: interestCategory.CreateAt
        );
    }

    public static UpdateInterestCategoryDTos MapUpdateInterestCategoryDTos(InterestCategory interestCategory)
    {
        return new UpdateInterestCategoryDTos
        (
            InterestCategoryId: interestCategory.Id,
            Name: interestCategory.Name!,
            UpdateAt: interestCategory.UpdateAt
        );
    }

    public static InterestCategoryGeneralDTos MapInterestCategoryGeneralDTos(InterestCategory interestCategory)
    {
        return new InterestCategoryGeneralDTos(
            InterestCategoryId: interestCategory.Id,
            Name: interestCategory.Name!,
            CreatedAt: interestCategory.CreateAt,
            UpdateAt: interestCategory.UpdateAt
        );
    }
}