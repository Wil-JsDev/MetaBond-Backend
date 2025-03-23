using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Events.Commands.Delete;

public sealed class DeleteEventsCommand : ICommand<Guid>
{
    public Guid Id { get; set; }
}