using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Query.GetById;

internal sealed class GetByIdNotificationQueryHandler(
    ILogger<GetByIdNotificationQueryHandler> logger,
    INotificationRepository notificationRepository
) : IQueryHandler<GetByIdNotificationQuery, NotificationDTos>
{
    public async Task<ResultT<NotificationDTos>> Handle(GetByIdNotificationQuery request,
        CancellationToken cancellationToken)
    {
        var notification = await EntityHelper.GetEntityByIdAsync(
            notificationRepository.GetByIdAsync,
            request.NotificationId ?? Guid.Empty,
            "Notification",
            logger
        );

        if (!notification.IsSuccess) return notification.Error;

        var notificationDto = NotificationMapper.MapNotificationDTos(notification.Value);

        logger.LogInformation("");

        return ResultT<NotificationDTos>.Success(notificationDto);
    }
}