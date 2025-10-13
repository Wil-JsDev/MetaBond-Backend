using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Interfaces.Service.SignaIR.Senders;

public interface INotificationSender
{
    Task SendNewNotificationAsync(Guid userId, NotificationDTos notification);

    Task SendNotificationsAsync(Guid userId, IEnumerable<NotificationDTos> notifications);

    Task SendNotificationReadAsync(Guid userId, Guid notificationId, NotificationDTos notification);

    Task SendNotificationDeletedAsync(Guid userId, Guid notificationId);

    Task SendAllNotificationsReadAsync(Guid userId, IEnumerable<NotificationDTos> notification);

    Task SendAllNotificationsDeletedAsync(Guid userId);

    Task SendNotificationUnreadCountAsync(Guid userId, int unreadCount);
}