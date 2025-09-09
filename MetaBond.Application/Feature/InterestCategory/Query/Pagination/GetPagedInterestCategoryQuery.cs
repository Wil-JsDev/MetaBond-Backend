using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.InterestCategory.Query.Pagination;

public sealed class GetPagedInterestCategoryQuery : IQuery<PagedResult<InterestCategoryGeneralDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}