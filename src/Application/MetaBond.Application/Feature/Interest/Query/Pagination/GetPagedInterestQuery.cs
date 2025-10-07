using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Interest.Query.Pagination;

public sealed class GetPagedInterestQuery : IQuery<PagedResult<InterestDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}