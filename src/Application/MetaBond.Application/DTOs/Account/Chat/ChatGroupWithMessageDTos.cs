using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Account.Chat;

public record ChatGroupWithMessageDTos(
    Guid ChatId,
    ChatType Type,
    string? Name,
    string? Photo,
    Guid? CommunityId,
    List<MessageWithChatDTos> Messages
) : ChatBaseWithMessageDTos();