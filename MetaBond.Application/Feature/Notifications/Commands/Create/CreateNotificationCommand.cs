using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Notifications.Commands.Create;

public sealed class CreateNotificationCommand : ICommand<NotificationDTos>
{
    public NotificationType Type { get; set; }
    
    public Guid? UserId { get; set; }
}