using MetaBond.Domain;

namespace MetaBond.Application.DTOs.Notifications;

public sealed record NotificationDTos(
    Guid? NotificationId,
    Guid? CreatedById,
    string? Content,
    NotificationType? NotificationType,
    DateTime? CreatedAt,
    bool? IsRead
);