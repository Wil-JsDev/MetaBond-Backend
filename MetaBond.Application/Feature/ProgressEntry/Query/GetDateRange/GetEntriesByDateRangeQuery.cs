using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetDateRange;

public sealed class GetEntriesByDateRangeQuery : IQuery<IEnumerable<ProgressEntryDTos>>
{
    public Guid ProgressBoardId { get; set; }
    public DateRangeType Range { get; set; }
}