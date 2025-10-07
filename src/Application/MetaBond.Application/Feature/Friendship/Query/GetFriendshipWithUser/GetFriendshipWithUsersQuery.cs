using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Friendship.Query.GetFriendshipWithUser;

public sealed class GetFriendshipWithUsersQuery : IQuery<PagedResult<FriendshipWithUserDTos>>
{
    public Guid? FriendshipId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}