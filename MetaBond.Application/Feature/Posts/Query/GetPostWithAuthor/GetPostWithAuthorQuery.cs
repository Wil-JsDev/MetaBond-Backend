using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;

public sealed class GetPostWithAuthorQuery : IQuery<IEnumerable<PostsWithUserDTos>>
{
    public Guid? PostsId { get; set; }
}