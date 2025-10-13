using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedAfter;

public sealed class GetCreatedAfterFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public DateRangeType DateRange { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}