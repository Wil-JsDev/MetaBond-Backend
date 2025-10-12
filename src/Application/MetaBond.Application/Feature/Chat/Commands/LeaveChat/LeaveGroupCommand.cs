using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;

namespace MetaBond.Application.Feature.Chat.Commands.LeaveChat;

public sealed class LeaveGroupCommand : ICommand<LeaveChatDTos>
{
    public Guid UserId { get; set; }

    public Guid ChatId { get; set; }
}