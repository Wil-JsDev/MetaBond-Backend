using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Communities.Query.GetCommunitiesByCategory;

public sealed class GetCommunitiesByCategoryIdQuery : IQuery<PagedResult<CommunitiesByCategoryDto>>
{
    public Guid CategoryId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}