using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAllAsRead;

public sealed class MarkAllAsReadNotificationCommand : ICommand<string>
{
    public Guid? UserId { get; set; }
}