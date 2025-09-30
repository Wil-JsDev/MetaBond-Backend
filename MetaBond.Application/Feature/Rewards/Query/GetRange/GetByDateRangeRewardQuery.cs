using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Rewards.Query.GetRange;

public sealed class GetByDateRangeRewardQuery : IQuery<PagedResult<RewardsDTos>>
{
    public DateRangeType Range { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}