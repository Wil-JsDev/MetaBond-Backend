using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;

namespace MetaBond.Application.Feature.Chat.Query.GetChatByUserId;

public sealed class GetChatByUserIdQuery : IQuery<ChatBaseWithMessageDTos>
{
    public Guid UserId { get; set; }

    public Guid ChatId { get; set; }
}