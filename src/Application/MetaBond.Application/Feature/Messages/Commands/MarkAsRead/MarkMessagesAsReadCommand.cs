using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Messages.Commands.MarkAsRead;

public sealed record MarkMessagesAsReadCommand(
    Guid ChatId,
    Guid UserId
) : ICommand;