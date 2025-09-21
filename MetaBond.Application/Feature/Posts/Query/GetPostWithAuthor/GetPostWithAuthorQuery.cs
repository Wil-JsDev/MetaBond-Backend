using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;

public sealed class GetPostWithAuthorQuery : IQuery<PagedResult<PostsWithUserDTos>>
{
    public Guid? PostsId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}