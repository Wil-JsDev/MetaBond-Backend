using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Rewards.Query.GetTop;

public sealed class GetTopRewardsQuery : IQuery<PagedResult<RewardsWithUserDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}