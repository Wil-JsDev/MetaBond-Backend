using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Rewards;

namespace MetaBond.Application.Feature.Rewards.Commands.Create;

public sealed class CreateRewardsCommand : ICommand<RewardsDTos>
{
    public string? Description { get; set; }

    public Guid UserId { get; set; }
    public int PointAwarded { get; set; }
}