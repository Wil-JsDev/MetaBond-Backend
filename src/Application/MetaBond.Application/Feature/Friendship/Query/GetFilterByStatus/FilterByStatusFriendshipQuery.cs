using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetFilterByStatus;

public sealed class FilterByStatusFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public Status Status { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}