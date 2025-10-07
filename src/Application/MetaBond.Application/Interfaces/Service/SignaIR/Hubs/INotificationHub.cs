using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Interfaces.Service.SignaIR.Hubs;

public interface INotificationHub
{
    Task OnNotificationNewReceived(NotificationDTos notification);

    Task OnNotificationsReceived(IEnumerable<NotificationDTos> notifications);

    Task OnNotificationRead(Guid notificationId, NotificationDTos notification);

    Task OnNotificationDeleted(Guid notificationId);

    Task OnAllNotificationsRead(IEnumerable<NotificationDTos> notification);

    Task OnAllNotificationsDeleted();

    Task OnNotificationUnreadCount(int unreadCount);
}