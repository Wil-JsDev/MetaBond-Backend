using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;

namespace MetaBond.Application.Feature.Chat.Commands.CreateChatPrivate;

public sealed class CreateChatPrivateCommand : ICommand<ChatPrivateDTos>
{
    public Guid UserId { get; set; }
}