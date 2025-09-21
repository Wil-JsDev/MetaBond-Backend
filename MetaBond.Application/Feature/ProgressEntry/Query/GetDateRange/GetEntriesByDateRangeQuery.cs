using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;

public sealed class GetEntriesByDateRangeQuery : IQuery<PagedResult<ProgressEntryDTos>>
{
    public Guid ProgressBoardId { get; set; }
    public DateRangeType Range { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}