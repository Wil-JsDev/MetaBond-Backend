using MetaBond.Application.DTOs.Notifications;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Mapper;

public static class NotificationMapper
{
    private const string Friendship = "Friendship";
    private const string PrivateMessage = "PrivateMessage";
    private const string FriendshipRequest = "FriendshipRequest";

    public static NotificationDTos MapNotificationDTos(Notification notification)
    {
        return new NotificationDTos(
            NotificationId: notification.Id,
            CreatedById: notification.UserId,
            Content: notification.Message,
            NotificationType: notification.Type switch
            {
                PrivateMessage => Domain.NotificationType.PrivateMessage,
                Friendship => Domain.NotificationType.Friendship,
                FriendshipRequest => Domain.NotificationType.FriendshipRequest,
                _ => null
            },
            CreatedAt: notification.CreatedAt,
            IsRead: notification.IsRead
        );
    }

    public static string? MessageNotification(string? username, NotificationType type)
    {
        return type switch
        {
            NotificationType.PrivateMessage => $"A private message has been sent to {username}.",
            NotificationType.FriendshipRequest => $"A friendship request has been sent to {username}.",
            NotificationType.Friendship => $"{username} has been added to your friends list.",
            _ => null
        };
    }

    public static NotificationWithUserDTos ToNotificationWithUserDto(Notification notification)
    {
        return new NotificationWithUserDTos(
            NotificationId: notification.Id,
            Content: notification.Message,
            NotificationType: Enum.TryParse<NotificationType>(notification.Type, out var parsedType)
                ? parsedType
                : null,
            CreatedAt: notification.CreatedAt,
            IsRead: notification.ReadAt.HasValue,
            CreatedBy: notification.User is null
                ? null
                : new UserNotificationDTos(
                    UserId: notification.User.Id,
                    FullName: $"{notification.User.FirstName} {notification.User.LastName}",
                    Email: notification.User.Email,
                    ImageUrl: notification.User.Photo
                )
        );
    }
}