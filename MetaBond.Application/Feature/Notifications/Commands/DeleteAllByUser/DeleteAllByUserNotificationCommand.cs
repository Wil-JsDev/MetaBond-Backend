using MetaBond.Application.Abstractions.Messaging;

namespace MetaBond.Application.Feature.Notifications.Commands.DeleteAllByUser;

public class DeleteAllByUserNotificationCommand : ICommand
{
    public Guid? UserId { get; set; }
}