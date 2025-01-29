using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Command.Create
{
    public sealed class CreateFriendshipCommand : ICommand<FriendshipDTos>
    {
        public Status Status { get; set; }
    }
}
