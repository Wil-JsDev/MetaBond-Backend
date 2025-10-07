using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;

public sealed class GetCreatedBeforeFriendshipQuery : IQuery<PagedResult<FriendshipDTos>>
{
    public PastDateRangeType PastDateRangeType { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}