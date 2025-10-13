using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationRecentByUser;

public sealed class GetNotificationRecentByUserQuery : IQuery<IEnumerable<NotificationWithUserDTos>>
{
    public Guid? UserId { get; set; }

    public int? Take { get; set; }
}