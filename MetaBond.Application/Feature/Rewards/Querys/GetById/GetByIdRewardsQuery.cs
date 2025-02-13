using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Querys.GetById
{
    public sealed class GetByIdRewardsQuery : IQuery<RewardsDTos>
    {
        public Guid RewardsId { get; set; }
    }
}
