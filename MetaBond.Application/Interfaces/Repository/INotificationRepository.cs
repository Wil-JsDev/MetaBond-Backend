using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

/// <summary>
/// Specialized repository for managing user notifications.
/// Inherits basic CRUD operations from <see cref="IGenericRepository{Notification}"/> 
/// and adds specific methods for querying and updating notifications.
/// </summary>
public interface INotificationRepository : IGenericRepository<Notification>
{
    /// <summary>
    /// Retrieves a paginated list of notifications for a given user.
    /// </summary>
    Task<PagedResult<Notification>> GetPagedNotificationsUserIdAsync(
        Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a specific notification for a user by its Id.
    /// </summary>
    Task<Notification?> GetNotificationByIdUserIdAsync(
        Guid notificationId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets the total count of unread notifications for a user.
    /// </summary>
    Task<int> GetUnreadCountByUserIdAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the most recent notifications for a user (not paginated).
    /// </summary>
    Task<IEnumerable<Notification>> GetRecentNotificationsByUserIdAsync(
        Guid userId, int take, CancellationToken cancellationToken);

    /// <summary>
    /// Marks a specific notification as read for a user.
    /// </summary>
    Task MarkAsReadAsync(
        Guid notificationId, Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Marks all notifications as read for a user.
    /// </summary>
    Task MarkAllAsReadAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes all notifications for a user.
    /// </summary>
    Task DeleteAllByUserIdAsync(
        Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves the next unread notification for a user, if any.
    /// </summary>
    Task<Notification?> GetNextUnreadByUserIdAsync(
        Guid userId, CancellationToken cancellationToken);
}