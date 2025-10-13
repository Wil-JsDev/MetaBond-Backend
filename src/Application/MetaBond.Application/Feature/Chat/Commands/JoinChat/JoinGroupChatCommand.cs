using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;

namespace MetaBond.Application.Feature.Chat.Commands.JoinChat;

public sealed class JoinGroupChatCommand : ICommand<ChatWithUserDTos>
{
    public Guid ChatId { get; set; }

    public Guid UserId { get; set; }
}