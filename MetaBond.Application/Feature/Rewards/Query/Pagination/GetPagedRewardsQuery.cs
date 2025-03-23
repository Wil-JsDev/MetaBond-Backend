using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Rewards.Query.Pagination;

public sealed class GetPagedRewardsQuery : IQuery<PagedResult<RewardsDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}