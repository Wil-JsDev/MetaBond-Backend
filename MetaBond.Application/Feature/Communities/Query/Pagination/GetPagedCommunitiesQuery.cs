using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Communities.Query.Pagination;

public sealed class GetPagedCommunitiesQuery : IQuery<PagedResult<CommunitiesDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}
    
