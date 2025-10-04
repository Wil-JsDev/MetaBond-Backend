using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAllAsRead;

internal sealed class MarkAllAsReadNotificationCommandHandler(
    ILogger<MarkAllAsReadNotificationCommandHandler> logger,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    INotificationSender notificationSender
) : ICommandHandler<MarkAllAsReadNotificationCommand, string>
{
    public async Task<ResultT<string>> Handle(MarkAllAsReadNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        await notificationRepository.MarkAllAsReadAsync(request.UserId ?? Guid.Empty, cancellationToken);

        logger.LogInformation(
            "MarkAllAsReadNotificationAsync: All notifications marked as read successfully for user ID '{UserId}'.",
            request.UserId);

        var notifications =
            await notificationRepository.GetAllByUserIdAsync(request.UserId ?? Guid.Empty, cancellationToken);

        var notificationMapped = notifications.Select(NotificationMapper.MapNotificationDTos);

        await notificationSender.SendAllNotificationsReadAsync(request.UserId ?? Guid.Empty,
            notificationMapped);

        logger.LogInformation(
            "MarkAllAsReadNotificationAsync: All notifications sent successfully for user ID '{UserId}'.",
            request.UserId);

        return ResultT<string>.Success("All notifications marked as read successfully.");
    }
}