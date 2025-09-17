using MetaBond.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaBond.Application.DTOs.Communities;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Communities.Query.GetPostsAndEvents;

public sealed class GetCommunityDetailsByIdQuery : IQuery<PagedResult<PostsAndEventsDTos>>
{
    public Guid Id { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}