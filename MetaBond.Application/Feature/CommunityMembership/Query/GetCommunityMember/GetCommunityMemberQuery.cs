using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.CommunityMembership;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.CommunityMembership.Query.GetCommunityMember;

public sealed class GetCommunityMemberQuery : IQuery<PagedResult<CommunityMembersDto>>
{
    public Guid? CommunityId { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}