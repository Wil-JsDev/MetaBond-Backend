using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetRange;

public sealed class GetRangeProgressBoardQuery : IQuery<IEnumerable<ProgressBoardWithProgressEntryDTos>>
{
    public int Page { get; set; }
        
    public int PageSize { get; set; }
    public DateRangeType DateRangeType {  get; set; }
}