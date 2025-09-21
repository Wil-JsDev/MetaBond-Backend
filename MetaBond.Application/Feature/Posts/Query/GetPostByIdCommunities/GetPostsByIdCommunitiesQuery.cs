using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Posts.Query.GetPostByIdCommunities;

public sealed class GetPostsByIdCommunitiesQuery : IQuery<PagedResult<PostsWithCommunitiesDTos>>
{
    public Guid PostsId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}