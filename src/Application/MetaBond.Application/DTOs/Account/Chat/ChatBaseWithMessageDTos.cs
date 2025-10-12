using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public abstract record ChatBaseWithMessageDTos
{
    public Guid ChatId { get; init; }

    public ChatType Type { get; init; }

    public List<MessageWithChatDTos> Messages { get; init; } = new();
}