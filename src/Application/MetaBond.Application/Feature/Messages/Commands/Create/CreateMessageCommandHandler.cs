using System.Reflection.Metadata;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.Message;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Messages.Commands.Create;

internal sealed class CreateMessageCommandHandler(
    IUserRepository userRepository,
    IMessageRepository messageRepository,
    IChatRepository chatRepository,
    ILogger<CreateMessageCommandHandler> logger
) : ICommandHandler<CreateMessageCommand, MessageGeneralDTos>
{
    public async Task<ResultT<MessageGeneralDTos>> Handle(CreateMessageCommand request,
        CancellationToken cancellationToken)
    {
        var userResult =
            await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync, request.SenderId, "User", logger);

        if (!userResult.IsSuccess) return ResultT<MessageGeneralDTos>.Failure(userResult.Error);

        var chatResult =
            await EntityHelper.GetEntityByIdAsync(chatRepository.GetByIdAsync, request.ChatId, "Chat", logger);

        if (!chatResult.IsSuccess) return ResultT<MessageGeneralDTos>.Failure(chatResult.Error);

        Message message = new()
        {
            Id = Guid.NewGuid(),
            ChatId = request.ChatId,
            SenderId = request.SenderId,
            Content = request.Content
        };

        await messageRepository.CreateAsync(message, cancellationToken);

        logger.LogInformation("Message created successfully");

        var messageDtos = MessageMapper.MapToMessageGeneralDTos(message, userResult.Value);

        return ResultT<MessageGeneralDTos>.Success(messageDtos);
    }
}