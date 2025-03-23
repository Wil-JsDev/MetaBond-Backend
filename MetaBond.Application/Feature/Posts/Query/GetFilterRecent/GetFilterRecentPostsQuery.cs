using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterRecent;

public sealed class GetFilterRecentPostsQuery : IQuery<IEnumerable<PostsDTos>>
{
    public Guid CommunitiesId { get; set; }
    public int TopCount { get; set; }
}