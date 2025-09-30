using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;

public sealed class GetRecentEntriesQuery : IQuery<PagedResult<ProgressEntryDTos>>
{
    public Guid ProgressBoardId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}