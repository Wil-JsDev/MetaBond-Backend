using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Command.Update
{
    public sealed class UpdateFriendshipCommand : ICommand<FriendshipDTos>
    {
        public Guid Id { get; set; } 

        public Status Status { get; set; }
    }
}
