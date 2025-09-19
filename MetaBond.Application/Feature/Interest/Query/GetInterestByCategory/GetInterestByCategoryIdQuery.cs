using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestByCategory;

public sealed class GetInterestByCategoryIdQuery : IQuery<PagedResult<InterestDTos>>
{
    public List<Guid>? InterestCategoryId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}