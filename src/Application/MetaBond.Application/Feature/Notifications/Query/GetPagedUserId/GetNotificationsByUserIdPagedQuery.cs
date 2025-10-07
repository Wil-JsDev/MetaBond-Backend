using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Pagination;

namespace MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;

public sealed class GetNotificationsByUserIdPagedQuery : IQuery<PagedResult<NotificationDTos>>
{
    public Guid? UserId { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}