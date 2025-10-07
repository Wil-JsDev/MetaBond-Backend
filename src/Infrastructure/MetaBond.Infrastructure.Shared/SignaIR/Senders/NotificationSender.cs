using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Interfaces.Service.SignaIR.Hubs;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Infrastructure.Shared.SignaIR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace MetaBond.Infrastructure.Shared.SignaIR.Senders;

public class NotificationSender(
    IHubContext<NotificationHub, INotificationHub> hubContext
) : INotificationSender
{
    public async Task SendNewNotificationAsync(Guid userId, NotificationDTos notification)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnNotificationNewReceived(notification);
    }

    public async Task SendNotificationsAsync(Guid userId, IEnumerable<NotificationDTos> notifications)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnNotificationsReceived(notifications);
    }

    public async Task SendNotificationReadAsync(Guid userId, Guid notificationId, NotificationDTos notification)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnNotificationRead(notificationId, notification);
    }

    public async Task SendNotificationDeletedAsync(Guid userId, Guid notificationId)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnNotificationDeleted(notificationId);
    }

    public async Task SendAllNotificationsReadAsync(Guid userId, IEnumerable<NotificationDTos> notification)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnAllNotificationsRead(notification);
    }

    public Task SendAllNotificationsDeletedAsync(Guid userId)
    {
        return hubContext.Clients.User(userId.ToString())
            .OnAllNotificationsDeleted();
    }

    public async Task SendNotificationUnreadCountAsync(Guid userId, int unreadCount)
    {
        await hubContext.Clients.User(userId.ToString())
            .OnNotificationUnreadCount(unreadCount);
    }
}