using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestsByName;

public sealed class GetInterestByNameQuery : IQuery<PagedResult<InterestWithUserDto>>
{
    public string? InterestName { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}