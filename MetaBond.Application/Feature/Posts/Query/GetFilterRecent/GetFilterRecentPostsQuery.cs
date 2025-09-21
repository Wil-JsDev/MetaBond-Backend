using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterRecent;

public sealed class GetFilterRecentPostsQuery : IQuery<PagedResult<PostsDTos>>
{
    public Guid CommunitiesId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}