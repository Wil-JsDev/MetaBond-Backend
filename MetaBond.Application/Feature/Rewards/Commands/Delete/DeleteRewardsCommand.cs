using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Rewards.Commands.Delete;

public sealed class DeleteRewardsCommand : ICommand<Guid>
{
    public Guid RewardsId { get; set; }
}