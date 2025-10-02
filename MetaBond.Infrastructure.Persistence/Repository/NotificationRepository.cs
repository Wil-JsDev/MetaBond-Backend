using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class NotificationRepository(MetaBondContext metaBondContext)
    : GenericRepository<Notification>(metaBondContext), INotificationRepository
{
    public async Task<PagedResult<Notification>> GetPagedNotificationsUserIdAsync(Guid userId, int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderByDescending(x => x.CreatedAt)
            .Select(n => new Notification
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                CreatedAt = n.CreatedAt,
                UserId = n.UserId,
                ReadAt = n.ReadAt,
                User = new User
                {
                    Id = n.User!.Id,
                    FirstName = n.User!.FirstName,
                    LastName = n.User!.LastName,
                    Email = n.User!.Email,
                    Photo = n.User!.Photo,
                }
            })
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedResult<Notification>(query, pageNumber, pageSize, total);
    }

    public async Task<Notification?> GetNotificationByIdUserIdAsync(Guid notificationId, Guid userId,
        CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .Include(nt => nt.User)
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId, cancellationToken);
    }

    public async Task<int> GetUnreadCountByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .CountAsync(x => x.UserId == userId && x.ReadAt == null, cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetRecentNotificationsByUserIdAsync(Guid userId, int take,
        CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new Notification
            {
                Id = n.Id,
                Message = n.Message,
                Type = n.Type,
                CreatedAt = n.CreatedAt,
                UserId = n.UserId,
                ReadAt = n.ReadAt,
                User = new User
                {
                    Id = n.User!.Id,
                    FirstName = n.User!.FirstName,
                    LastName = n.User!.LastName,
                    Email = n.User!.Email,
                    Photo = n.User!.Photo,
                }
            })
            .Take(take)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken cancellationToken)
    {
        var notification = await GetNotificationByIdUserIdAsync(notificationId, userId, cancellationToken);

        notification!.ReadAt = DateTime.UtcNow;

        await SaveAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.ReadAt == null)
            .ExecuteUpdateAsync(n => n.SetProperty(nt => nt.ReadAt, DateTime.UtcNow), cancellationToken);
    }

    public async Task DeleteAllByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _metaBondContext.Set<Notification>()
            .Where(n => n.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<Notification?> GetNextUnreadByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Notification>()
            .AsNoTracking()
            .Where(n => n.UserId == userId && n.ReadAt == null)
            .OrderBy(n => n.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}