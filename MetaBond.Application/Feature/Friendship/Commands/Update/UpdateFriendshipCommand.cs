using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Commands.Update;

public sealed class UpdateFriendshipCommand : ICommand<UpdateFriendshipDTos>
{
    public Guid Id { get; set; } 

    public Status Status { get; set; }
}