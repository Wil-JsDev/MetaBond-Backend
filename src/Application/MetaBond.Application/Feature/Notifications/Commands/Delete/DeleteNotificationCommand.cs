using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Notifications.Commands.Delete;

public sealed class DeleteNotificationCommand : ICommand
{
    public Guid? NotificationId { get; set; }

    public Guid? UserId { get; set; }
}