using MetaBond.Application.DTOs.Account.User;
using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Notifications;

public sealed record NotificationWithUserDTos(
    Guid? NotificationId,
    string? Content,
    NotificationType? NotificationType,
    DateTime? CreatedAt,
    bool? IsRead,
    UserNotificationDTos? CreatedBy
);

public record UserNotificationDTos(
    Guid UserId,
    string? FullName,
    string? Email,
    string? ImageUrl
);