using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.CommunityCategory;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.CommunityCategory.Query.Pagination;

public sealed class GetPagedCommunityCategoryQuery : IQuery<PagedResult<CommunityCategoryDTos>>
{
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}