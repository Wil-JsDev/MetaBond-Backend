using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Friendship.Commands.Delete;

public sealed class DeleteFriendshipCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
}