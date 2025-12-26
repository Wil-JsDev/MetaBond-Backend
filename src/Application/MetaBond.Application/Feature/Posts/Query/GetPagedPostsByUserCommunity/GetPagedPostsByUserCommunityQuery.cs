using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Posts.Query.GetPagedPostsByUserCommunity;

public sealed class GetPagedPostsByUserCommunityQuery : IQuery<PagedResult<PostsDTos>>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public Guid? CommunitiesId { get; set; }

    public Guid? CreatedById { get; set; }
}