using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Messages.Commands.MarkAsRead;

internal sealed class MarkMessagesAsReadCommandHandler(
    IMessageRepository messageRepository,
    IMessageReadRepository messageReadRepository,
    IChatRepository chatRepository,
    IChatSender chatSender,
    IUserRepository userRepository,
    ICurrentService currentService,
    ILogger<MarkMessagesAsReadCommandHandler> logger)
    : ICommandHandler<MarkMessagesAsReadCommand>
{
    public async Task<Result> Handle(
        MarkMessagesAsReadCommand request,
        CancellationToken cancellationToken)
    {
        var chat = await EntityHelper.GetEntityByIdAsync(
            chatRepository.GetByIdAsync,
            request.ChatId,
            "Chat",
            logger);

        if (!chat.IsSuccess)
            return Result.Failure(chat.Error!);

        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger);

        if (!user.IsSuccess)
            return Result.Failure(user.Error!);

        if (!await chatRepository.IsUserInChatAsync(
                request.ChatId,
                request.UserId,
                cancellationToken))
        {
            return Result.Failure(
                Error.Forbidden("403", "User is not a member of the chat."));
        }

        if (request.UserId != currentService.CurrentId)
        {
            return Result.Failure(
                Error.Forbidden(
                    "403",
                    "You are not allowed to perform this action on behalf of another user."
                ));
        }

        var unreadMessages = await messageRepository.GetUnreadMessagesAsync(
            request.ChatId,
            request.UserId,
            cancellationToken);

        if (!unreadMessages.Any())
        {
            logger.LogInformation(
                "No unread messages found for ChatId {ChatId} and UserId {UserId}",
                request.ChatId,
                request.UserId);

            return Result.Success();
        }

        var messageReads = unreadMessages.Select(message => new MessageRead
        {
            MessageId = message.Id,
            UserId = request.UserId,
            ReadAt = DateTime.UtcNow
        }).ToList();

        await messageReadRepository.CreateRangeAsync(
            messageReads,
            cancellationToken);

        await chatSender.SendMessagesReadAsync(
            request.ChatId,
            request.UserId,
            messageReads.Select(mr => mr.MessageId).ToList());

        return Result.Success();
    }
}