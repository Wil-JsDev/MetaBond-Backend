using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterRecent
{
    public sealed class GetFilterRecentPostsQuery : IQuery<IEnumerable<PostsDTos>>
    {
        public int TopCount { get; set; }
    }
}
