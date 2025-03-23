using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;

public sealed class GetProgressBoardIdWithEntriesQuery : IQuery<IEnumerable<ProgressBoardWithProgressEntryDTos>>
{
    public int PageNumber { get; set; }
        
    public int PageSize { get; set; }
    public Guid ProgressBoardId { get; set; }
}