using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public sealed record ChatPrivateWithMessageDTos(
    Guid ChatId,
    ChatType Type,
    List<MessageWithChatDTos> Messages
) : ChatBaseWithMessageDTos();