using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.Pagination;

public sealed class GetPagedParticipationInEventQuery : IQuery<PagedResult<ParticipationInEventDTos>>
{
    public int PageSize { get; set; }

    public int PageNumber { get; set; }
}