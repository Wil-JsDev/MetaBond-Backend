using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Friendship.Query.Pagination;

public sealed class GetPagedFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}