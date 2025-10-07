using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Commands.Update;

public sealed class UpdateRewardsCommand : ICommand<RewardsDTos>
{
    public Guid RewardsId { get; set; }

    public string? Description { get; set; }

    public int PointAwarded { get; set; }
}