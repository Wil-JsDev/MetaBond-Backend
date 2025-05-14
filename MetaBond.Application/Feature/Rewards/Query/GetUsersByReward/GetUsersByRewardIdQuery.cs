using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Query.GetUsersByReward;

public sealed class GetUsersByRewardIdQuery : IQuery<IEnumerable<RewardsWithUserDTos>>
{
    public Guid RewardsId { get; init; }
}