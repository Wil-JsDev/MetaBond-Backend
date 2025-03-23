using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTitle;

public sealed class GetFilterTitlePostsQuery : IQuery<IEnumerable<PostsDTos>>
{
    public Guid CommunitiesId { get; set; }
    public string? Title { get; set; }
}