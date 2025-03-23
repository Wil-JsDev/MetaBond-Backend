using System.Collections;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetByIdProgressEntryWithProgressBoard;

public class GetProgressEntryWithBoardByIdQuery : IQuery<IEnumerable<ProgressEntryWithProgressBoardDTos>>
{
    public Guid ProgressEntryId { get; set; }
}