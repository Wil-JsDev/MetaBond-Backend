using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Chat.Query.GetUserInChat;

public sealed class GetUserByChatIdQuery : IQuery<PagedResult<UserChatDTos>>
{
    public Guid ChatId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}