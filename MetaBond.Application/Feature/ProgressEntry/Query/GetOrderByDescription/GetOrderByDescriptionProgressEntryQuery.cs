
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderByDescription
{
    public sealed class GetOrderByDescriptionProgressEntryQuery : IQuery<IEnumerable<ProgressEntryWithDescriptionDTos>>
    {
        public Guid ProgressBoardId { get; set; }
    }
}
