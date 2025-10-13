using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Update;

public sealed class UpdateProgressEntryCommand : ICommand<ProgressEntryDTos>
{
    public Guid ProgressEntryId { get; set; }

    public string? Description { get; set; }
}