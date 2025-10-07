using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class CommunityCategoryMapper
{
    public static CommunityCategoryDTos MapCommunityCategoryDTos(CommunityCategory communityCategory)
    {
        return new CommunityCategoryDTos
        (
            CommunityCategoryId: communityCategory.Id,
            Name: communityCategory.Name!,
            CreatedAt: communityCategory.CreateAt,
            UpdateAt: communityCategory.UpdateAt
        );
    }
}