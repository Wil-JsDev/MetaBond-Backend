using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Querys.GetById
{
    public sealed class GetByIdProgressEntryQuery : IQuery<ProgressEntryDTos>
    {
        public Guid ProgressEntryId { get; set; }
    }
}
