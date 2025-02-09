using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Create
{
    public sealed class CreateProgressEntryCommand : ICommand<ProgressEntryDTos>
    {
        public Guid ProgressBoardId { get; set; }

        public string? Description { get; set; }
    
    }
}
