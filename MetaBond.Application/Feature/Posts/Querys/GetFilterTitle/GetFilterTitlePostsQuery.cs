using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterTitle
{
    public sealed class GetFilterTitlePostsQuery : IQuery<IEnumerable<PostsDTos>>
    {
        public string? Title { get; set; }
    }
}
