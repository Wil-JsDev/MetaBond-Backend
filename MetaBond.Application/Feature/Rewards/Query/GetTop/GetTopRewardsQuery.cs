using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Querys.GetTop
{
    public sealed class GetTopRewardsQuery : IQuery<IEnumerable<RewardsDTos>>
    {
        public int TopCount { get; set; }
    }
}
