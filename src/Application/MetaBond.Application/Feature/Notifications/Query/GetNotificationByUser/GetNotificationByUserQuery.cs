using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationByUser;

public sealed class GetNotificationByUserQuery : IQuery<NotificationWithUserDTos>
{
    public Guid? UserId { get; set; }

    public Guid? NotificationId { get; set; }
}