using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;

public class GetProgressEntryWithBoardByIdQuery : IQuery<IEnumerable<ProgressEntryWithProgressBoardDTos>>
{
    public Guid ProgressEntryId { get; set; }
}