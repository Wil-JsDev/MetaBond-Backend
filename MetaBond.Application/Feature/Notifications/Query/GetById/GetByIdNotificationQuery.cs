using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Feature.Notifications.Query.GetById;

public sealed class GetByIdNotificationQuery : IQuery<NotificationDTos>
{
    public Guid? NotificationId { get; set; }
}