using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Base;

namespace MetaBond.Application.Feature.Notifications.Query.GetUnreadCount;

public sealed class GetNotificationUnreadCountQuery : IQuery<CountResponse>
{
    public Guid? UserId { get; set; }
}