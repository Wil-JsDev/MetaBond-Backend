using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class ChatRepository(MetaBondContext metaBondContext) : GenericRepository<Chat>(metaBondContext), IChatRepository
{
    public async Task<PagedResult<Chat>> GetPagedChatByUserIdAsync(int pageNumber, int pageSize, Guid userId,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Chat>()
            .AsNoTracking()
            .Where(ch => ch.UserChats.Any(uc => uc.UserId == userId));

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(ch => ch.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Chat>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<Chat?> GetByIdAndUserIdAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Chat>()
            .AsNoTracking()
            .Where(ch => ch.Id == chatId && ch.UserChats.Any(uc => uc.UserId == userId))
            .Include(ch => ch.UserChats)
            .ThenInclude(uc => uc.User)
            .Include(ch => ch.Messages)
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> IsUserInChatAsync(Guid chatId, Guid userId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us => us.Id == chatId && us.UserChats.Any(uc => uc.UserId == userId),
            cancellationToken);
    }

    public async Task<PagedResult<Chat>> GetChatsWithLastMessageAsync(Guid userId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Chat>()
            .AsNoTracking()
            .Where(ch => ch.UserChats.Any(us => us.UserId == userId));

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderByDescending(ch => ch.LastMessageAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Chat>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<PagedResult<Chat>> GetGroupChatsByCommunityIdAsync(Guid communityId, int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Chat>()
            .AsNoTracking()
            .Where(ch => ch.CommunityId == communityId && ch.Type == ChatType.CommunityGroup.ToString());

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(ch => ch.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Chat>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }

    public async Task<bool> IsCommunityGroupChatExistAsync(Guid communityId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(ch => ch.CommunityId == communityId &&
                                         ch.Type == ChatType.CommunityGroup.ToString(), cancellationToken);
    }

    public async Task<bool> HasAnyChatAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(ch => ch.UserChats.Any(uc => uc.UserId == userId), cancellationToken);
    }

    public async Task<Chat?> GetDirectChatBetweenUsersAsync(Guid userAId, Guid userBId,
        CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<Chat>()
            .AsNoTracking()
            .Where(ch => ch.Type == ChatType.Direct.ToString() &&
                         ch.UserChats.Any(uc => uc.UserId == userAId) &&
                         ch.UserChats.Any(uc => uc.UserId == userBId))
            .Include(ch => ch.UserChats)
            .ThenInclude(uc => uc.User)
            .Include(ch => ch.Messages)
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasParticipantsAsync(Guid chatId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(ch => ch.UserChats.Any(uh => uh.ChatId == chatId), cancellationToken);
    }

    public async Task<PagedResult<User>> GetUsersInChatAsync(Guid chatId, int pageSize, int pageNumber,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<User>()
            .AsNoTracking()
            .Where(us => us.UserChats != null && us.UserChats.Any(uc => uc.ChatId == chatId));

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = await baseQuery
            .OrderBy(us => us.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<User>(query, pageNumber, pageSize, total);

        return pagedResponse;
    }
}