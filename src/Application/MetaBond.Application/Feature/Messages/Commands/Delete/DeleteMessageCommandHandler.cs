using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Messages.Commands.Delete;

internal sealed class DeleteMessageCommandHandler(
    ILogger<DeleteMessageCommandHandler> logger,
    IMessageRepository messageRepository
) : ICommandHandler<DeleteMessageCommand>
{
    public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await EntityHelper.GetEntityByIdAsync(
            messageRepository.GetByIdAsync,
            request.MessageId,
            "Message",
            logger);

        if (!message.IsSuccess) return Result.Failure(message.Error!);

        message.Value.IsDeleted = true;

        await messageRepository.DeleteAsync(message.Value, cancellationToken);

        logger.LogInformation("Message deleted successfully");

        return Result.Success();
    }
}