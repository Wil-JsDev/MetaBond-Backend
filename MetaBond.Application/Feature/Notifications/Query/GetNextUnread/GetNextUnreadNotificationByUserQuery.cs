using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Feature.Notifications.Query.GetNextUnread;

public sealed class GetNextUnreadNotificationByUserQuery : IQuery<NotificationDTos>
{
    public Guid? UserId { get; set; }
}