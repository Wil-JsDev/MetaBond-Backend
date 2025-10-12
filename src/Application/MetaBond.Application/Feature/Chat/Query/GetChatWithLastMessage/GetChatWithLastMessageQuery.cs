using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Chat.Query.GetChatWithLastMessage;

public sealed class GetChatWithLastMessageQuery : IQuery<PagedResult<ChatWitLastMessageDTos>>
{
    public Guid UserId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}