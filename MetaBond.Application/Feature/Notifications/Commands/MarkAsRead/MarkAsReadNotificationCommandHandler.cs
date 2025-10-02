using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAsRead;

internal sealed class MarkAsReadNotificationCommandHandler(
    ILogger<MarkAsReadNotificationCommandHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository
) : ICommandHandler<MarkAsReadNotificationCommand, NotificationDTos>
{
    public async Task<ResultT<NotificationDTos>> Handle(MarkAsReadNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error;

        var notification = await EntityHelper.GetEntityByIdAsync(
            notificationRepository.GetByIdAsync,
            request.NotificationId ?? Guid.Empty,
            "Notification",
            logger
        );

        if (!notification.IsSuccess) return notification.Error;

        if (notification.Value.UserId != user.Value.Id)
        {
            const string errorMessage = "The notification does not belong to the specified user.";

            logger.LogWarning(message: errorMessage);

            return ResultT<NotificationDTos>.Failure(Error.Conflict("409", errorMessage));
        }

        await notificationRepository.MarkAsReadAsync(request.NotificationId ?? Guid.Empty, request.UserId ?? Guid.Empty,
            cancellationToken);

        logger.LogInformation(
            "MarkAsReadNotificationAsync: Notification marked as read successfully for user ID '{UserId}'.",
            request.UserId);

        var notificationDto = NotificationMapper.MapNotificationDTos(notification.Value);

        return ResultT<NotificationDTos>.Success(notificationDto);
    }
}