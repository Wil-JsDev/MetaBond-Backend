
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetDateRange
{
    public sealed class GetEntriesByDateRangeQuery : IQuery<IEnumerable<ProgressEntryDTos>>
    {
        public DateRangeType Range {  get; set; }
    }
}
