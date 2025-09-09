using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;

namespace MetaBond.Application.Feature.CommunityCategory.Query.GetById;

public sealed class GetByIdCommunityCategoryQuery : IQuery<CommunityCategoryDTos>
{
    public Guid CommunityCategoryId { get; set; }
}