using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Rewards.Query.GetRange;

public sealed class GetByDateRangeRewardQuery : IQuery<IEnumerable<RewardsDTos>>
{
    public DateRangeType Range {  get; set; }
}