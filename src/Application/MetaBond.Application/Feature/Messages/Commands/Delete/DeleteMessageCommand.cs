using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Messages.Commands.Delete;

public sealed class DeleteMessageCommand : ICommand
{
    public Guid MessageId { get; set; }
}