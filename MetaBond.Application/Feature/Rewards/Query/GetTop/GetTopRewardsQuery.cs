using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Query.GetTop;

public sealed class GetTopRewardsQuery : IQuery<IEnumerable<RewardsWithUserDTos>>
{
    public int TopCount { get; set; }
}