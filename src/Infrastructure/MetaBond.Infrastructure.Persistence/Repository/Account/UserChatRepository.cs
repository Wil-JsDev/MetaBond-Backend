using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class UserChatRepository(MetaBondContext metaBondContext)
    : GenericRepository<UserChat>(metaBondContext), IUserChatRepository
{
    public async Task<UserChat?> GetByUserIdAndChatIdAsync(Guid userId, Guid chatId,
        CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<UserChat>()
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.ChatId == chatId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task AddUserToChatAsync(UserChat userChat, CancellationToken cancellationToken)
    {
        await CreateAsync(userChat, cancellationToken);
    }

    public async Task<PagedResult<UserChat>> GetChatsByUserIdAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<UserChat>()
            .AsNoTracking()
            .Where(uc => uc.UserId == userId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(uc => uc.ChatId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(uc => uc.Chat)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<UserChat>(query, 1, total, total);

        return pagedResponse;
    }

    public async Task<PagedResult<UserChat>> GetUsersByChatIdAsync(Guid chatId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<UserChat>()
            .AsNoTracking()
            .Where(uc => uc.ChatId == chatId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(uc => uc.UserId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Include(uc => uc.User)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new PagedResult<UserChat>(query, pageNumber, pageSize, total);
    }

    public async Task RemoveUserFromChatAsync(Guid userId, Guid chatId, CancellationToken cancellationToken)
    {
        var userChat = await _metaBondContext.Set<UserChat>()
            .Where(uc => uc.UserId == userId && uc.ChatId == chatId)
            .FirstOrDefaultAsync(cancellationToken);

        if (userChat != null)
        {
            await DeleteAsync(userChat, cancellationToken);
        }
    }
}