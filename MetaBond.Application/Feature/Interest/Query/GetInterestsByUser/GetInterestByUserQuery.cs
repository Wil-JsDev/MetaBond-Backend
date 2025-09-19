using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestsByUser;

public sealed class GetInterestByUserQuery : IQuery<PagedResult<InterestDTos>>
{
    public Guid UserId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}