using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetOrderById
{
    public sealed class GetOrderByIdProgressEntryQuery : IQuery<IEnumerable<ProgressEntryBasicDTos>>
    {
        public Guid ProgressBoardId { get; set; }
    }
}
