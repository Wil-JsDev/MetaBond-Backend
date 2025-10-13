using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Query.GetNextUnread;

internal sealed class GetNextUnreadNotificationByUserQueryHandler(
    ILogger<GetNextUnreadNotificationByUserQueryHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository
) : IQueryHandler<GetNextUnreadNotificationByUserQuery, NotificationDTos>
{
    public async Task<ResultT<NotificationDTos>> Handle(GetNextUnreadNotificationByUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error;

        var notification =
            await notificationRepository.GetNextUnreadByUserIdAsync(request.UserId ?? Guid.Empty, cancellationToken);

        if (notification is null)
        {
            logger.LogWarning(
                "GetNextUnreadNotificationByUserQueryHandler: No unread notification found for user ID '{UserId}'.",
                request.UserId);

            return ResultT<NotificationDTos>.Failure(Error.NotFound("404", "No unread notification found"));
        }

        logger.LogInformation(
            "GetNextUnreadNotificationByUserQueryHandler: Found unread notification for user ID '{UserId}'.",
            request.UserId);

        var notificationDto = NotificationMapper.MapNotificationDTos(notification);

        return ResultT<NotificationDTos>.Success(notificationDto);
    }
}