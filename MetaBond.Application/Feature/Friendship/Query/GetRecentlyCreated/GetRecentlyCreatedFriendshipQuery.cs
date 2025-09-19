using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;

public sealed class GetRecentlyCreatedFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public int Limit { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}