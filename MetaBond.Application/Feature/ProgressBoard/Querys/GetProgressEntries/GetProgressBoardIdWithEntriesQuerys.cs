using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;

namespace MetaBond.Application.Feature.ProgressBoard.Querys.GetProgressEntries
{
    public sealed class GetProgressBoardIdWithEntriesQuerys : IQuery<IEnumerable<ProgressBoardWithProgressEntryDTos>>
    {
        public Guid ProgressBoardId { get; set; }
    }
}
