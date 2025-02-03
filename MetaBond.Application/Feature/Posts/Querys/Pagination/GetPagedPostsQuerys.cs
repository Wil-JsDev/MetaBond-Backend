using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Posts.Querys.Pagination
{
    public sealed class GetPagedPostsQuerys : IQuery<PagedResult<PostsDTos>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
