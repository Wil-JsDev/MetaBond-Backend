using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Friendship.Query.GetOrderById;

public sealed class GetOrderByIdFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public string? Sort { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}