using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Communties;
using MetaBond.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Communities.Querys.Pagination
{
    public sealed class GetPagedCommunitiesQuery : IQuery<PagedResult<CommunitiesDTos>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }  
    }
}
