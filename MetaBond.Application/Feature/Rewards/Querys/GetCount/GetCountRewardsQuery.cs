
using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Rewards.Querys.GetCount
{
    public sealed class GetCountRewardsQuery : IQuery<int>
    {
        public Guid RewardsId { get; set; }
    }
}
