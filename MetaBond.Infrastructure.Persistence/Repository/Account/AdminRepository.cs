using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class AdminRepository(MetaBondContext metaBondContext, IUserRepository userRepository)
    : GenericRepository<Admin>(metaBondContext), IAdminRepository
{
    public async Task<PagedResult<Admin>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .CountAsync(cancellationToken);

        var pagedAdmin = await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Admin>(pagedAdmin, pageNumber, pageSize, totalRecord);
    }

    public async Task<Admin?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email,
                cancellationToken);
    }

    public async Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Admin>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username,
                cancellationToken);
    }

    public async Task<bool> ExistsEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us => us.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us => us.Username == username, cancellationToken);
    }

    public async Task<PagedResult<User>> GetPagedUserStatusAsync(StatusAccount statusAccount, int pageNumber,
        int pageSize, CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<User>()
            .AsNoTracking()
            .Where(user => user.StatusUser == statusAccount.ToString());

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(user => user.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(query, pageNumber, pageSize, total);
    }

    public async Task<User> BanUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId);

        user!.StatusUser = StatusAccount.Banned.ToString();

        user.BannedAt = DateTime.UtcNow;

        await userRepository.UpdateAsync(user, cancellationToken);

        return user;
    }

    public async Task<User> UnbanUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(userId);

        user!.StatusUser = StatusAccount.Active.ToString();

        await userRepository.UpdateAsync(user, cancellationToken);

        return user;
    }
}