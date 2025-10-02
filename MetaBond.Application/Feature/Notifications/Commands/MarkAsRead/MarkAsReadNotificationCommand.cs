using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAsRead;

public sealed class MarkAsReadNotificationCommand : ICommand<NotificationDTos>
{
    public Guid? NotificationId { get; set; }
    public Guid? UserId { get; set; }
}