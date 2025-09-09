using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;

namespace MetaBond.Application.Feature.CommunityCategory.Query.GetByName;

public sealed class GetByNameCommunityCategoryQuery : IQuery<CommunityCategoryDTos>
{
    public string? Name { get; set; }
}