using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Friendship.Query.GetCreated.GetCreatedBefore;

public sealed class GetCreatedBeforeFriendshipQuery : IQuery<IEnumerable<FriendshipDTos>>
{
    public PastDateRangeType PastDateRangeType {  get; set; }
}