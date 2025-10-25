using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Messages.Commands.UpdateEdited;

internal sealed class UpdateEditedMessageCommandHandler(
    ILogger<UpdateEditedMessageCommandHandler> logger,
    IMessageRepository messageRepository
) : ICommandHandler<UpdateEditedMessageCommand, UpdateEditedMessageDTos>
{
    public async Task<ResultT<UpdateEditedMessageDTos>> Handle(UpdateEditedMessageCommand request,
        CancellationToken cancellationToken)
    {
        var message =
            await EntityHelper.GetEntityByIdAsync(messageRepository.GetByIdAsync, request.MessageId, "Message", logger);

        if (!message.IsSuccess) return ResultT<UpdateEditedMessageDTos>.Failure(message.Error!);

        message.Value.Content = request.Content;
        message.Value.IsEdited = true;
        message.Value.EditedAt = DateTime.UtcNow;

        await messageRepository.UpdateAsync(message.Value, cancellationToken);

        var updatedMessageDto = new UpdateEditedMessageDTos(message.Value.Content ?? string.Empty);

        return ResultT<UpdateEditedMessageDTos>.Success(updatedMessageDto);
    }
}