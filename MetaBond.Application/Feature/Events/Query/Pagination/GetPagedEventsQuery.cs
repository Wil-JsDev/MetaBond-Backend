using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Events.Query.Pagination
{
    public sealed class GetPagedEventsQuery : IQuery<PagedResult<EventsDto>>
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }
}
