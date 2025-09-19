using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetRecent;

public sealed class GetRecentProgressBoardQuery : IQuery<IEnumerable<ProgressBoardDTos>>
{
    public DateRangeFilter DateFilter { get; set; }
}