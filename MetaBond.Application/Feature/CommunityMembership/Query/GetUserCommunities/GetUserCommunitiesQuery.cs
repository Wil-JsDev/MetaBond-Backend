using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.CommunityMembership.Query.GetUserCommunities;

public sealed class GetUserCommunitiesQuery : IQuery<PagedResult<CommunitiesDTos>>
{
    public Guid? UserId { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}