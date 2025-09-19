using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Events.Query.GetCommunities;

public sealed class GetEventsDetailsQuery : IQuery<PagedResult<CommunitiesEventsDTos>>
{
    public Guid Id { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}