using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationByUser;

internal sealed class GetNotificationByUserQueryHandler(
    ILogger<GetNotificationByUserQueryHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository
) : IQueryHandler<GetNotificationByUserQuery, NotificationWithUserDTos>
{
    public async Task<ResultT<NotificationWithUserDTos>> Handle(GetNotificationByUserQuery request,
        CancellationToken cancellationToken)
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

        var notificationByUser =
            await notificationRepository.GetNotificationByIdUserIdAsync(request.NotificationId ?? Guid.Empty,
                request.UserId ?? Guid.Empty, cancellationToken);

        if (notificationByUser is null)
        {
            logger.LogWarning(
                "GetNotificationByUserQueryHandler: Notification not found for user ID '{UserId}' and notification ID '{NotificationId}'.",
                request.UserId, request.NotificationId);

            return ResultT<NotificationWithUserDTos>.Failure(Error.NotFound("404", "Notification not found"));
        }

        logger.LogInformation(
            "GetNotificationByUserQueryHandler: Notification found for user ID '{UserId}' and notification ID '{NotificationId}'.",
            request.UserId, request.NotificationId);

        var notificationDto = NotificationMapper.ToNotificationWithUserDto(notificationByUser);

        return ResultT<NotificationWithUserDTos>.Success(notificationDto);
    }
}