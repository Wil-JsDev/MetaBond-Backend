using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Communities.Query.Filter;

public sealed class GetCommunitiesByCategoryIdQuery : IQuery<PagedResult<CommunitiesDTos>>
{
    public Guid CategoryId { get; set; }
    
    public int PageNumber { get; set; }
    
    public int PageSize { get; set; }
}