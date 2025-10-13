using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;

public class GetProgressEntryWithBoardByIdQuery : IQuery<PagedResult<ProgressEntryWithProgressBoardDTos>>
{
    public Guid ProgressEntryId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}