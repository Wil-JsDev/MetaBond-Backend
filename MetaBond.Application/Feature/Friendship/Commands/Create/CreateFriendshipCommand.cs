﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Commands.Create;

public sealed class CreateFriendshipCommand : ICommand<FriendshipDTos>
{
    public Status Status { get; set; }

    public Guid? RequesterId { get; set; }

    public Guid? AddresseeId { get; set; }
}