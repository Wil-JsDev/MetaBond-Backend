using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Pagination;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;

public sealed class GetRecentProgressBoardQuery : IQuery<PagedResult<ProgressBoardDTos>>
{
    public DateRangeFilter DateFilter { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}