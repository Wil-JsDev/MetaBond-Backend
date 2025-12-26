using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;

namespace MetaBond.Application.Feature.Messages.Commands.UpdateEdited;

public sealed class UpdateEditedMessageCommand : ICommand<UpdateEditedMessageDTos>
{
    public Guid MessageId { get; set; }

    public required string Content { get; set; }
}