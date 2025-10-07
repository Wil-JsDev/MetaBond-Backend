using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderByDescription;

public sealed class GetOrderByDescriptionProgressEntryQuery : IQuery<PagedResult<ProgressEntryWithDescriptionDTos>>
{
    public Guid ProgressBoardId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}