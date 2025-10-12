using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Chat;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Chat.Query.GetGroupChatsByCommunity;

public sealed class GetGroupChatsByCommunityQuery : IQuery<PagedResult<ChatDTos>>
{
    public Guid CommunityId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}