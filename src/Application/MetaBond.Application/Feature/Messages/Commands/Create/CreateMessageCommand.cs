using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;

namespace MetaBond.Application.Feature.Messages.Commands.Create;

public sealed class CreateMessageCommand : ICommand<MessageGeneralDTos>
{
    public Guid SenderId { get; set; }

    public Guid ChatId { get; set; }

    public required string Content { get; set; }
}