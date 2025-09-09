namespace MetaBond.Application.DTOs.CommunityCategory;

public sealed record CommunityCategoryDTos
(
    Guid CommunityCategoryId,
    string Name,
    DateTime CreatedAt,
    DateTime? UpdateAt
);