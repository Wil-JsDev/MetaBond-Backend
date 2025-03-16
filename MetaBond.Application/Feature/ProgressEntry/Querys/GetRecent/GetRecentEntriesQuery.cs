using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetRecent
{
    public sealed class GetRecentEntriesQuery : IQuery<IEnumerable<ProgressEntryDTos>>
    {
        public Guid ProgressBoardId { get; set; }
        public int TopCount { get; set; }
    }
}
