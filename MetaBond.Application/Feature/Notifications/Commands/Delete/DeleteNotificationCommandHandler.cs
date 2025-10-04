using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Commands.Delete;

internal sealed class DeleteNotificationCommandHandler(
    ILogger<DeleteNotificationCommandHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    INotificationSender notificationSender
) : ICommandHandler<DeleteNotificationCommand>
{
    public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        var notification = await EntityHelper.GetEntityByIdAsync(
            notificationRepository.GetByIdAsync,
            request.NotificationId ?? Guid.Empty,
            "Notification",
            logger
        );

        if (!notification.IsSuccess) return notification.Error;

        await notificationRepository.DeleteAsync(notification.Value, cancellationToken);

        logger.LogInformation(
            "DeleteNotificationAsync: Notification deleted successfully for user ID '{UserId}' and notification ID '{NotificationId}'.",
            request.UserId, request.NotificationId);

        await notificationSender.SendNotificationDeletedAsync(request.UserId ?? Guid.Empty,
            request.NotificationId ?? Guid.Empty);

        logger.LogInformation(
            "DeleteNotificationAsync: Notification sent successfully for user ID '{UserId}'.",
            request.UserId);

        return Result.Success();
    }
}