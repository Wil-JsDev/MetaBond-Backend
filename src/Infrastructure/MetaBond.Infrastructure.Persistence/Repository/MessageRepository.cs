using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class MessageRepository(MetaBondContext metaBondContext)
    : GenericRepository<Message>(metaBondContext), IMessageRepository
{
    public async Task<PagedResult<Message>> GetPagedMessagesAsync(int pageNumber, int pageSize, Guid chatId,
        CancellationToken cancellationToken)
    {
        var baseQuery = _metaBondContext.Set<Message>()
            .AsNoTracking()
            .Where(ms => ms.ChatId == chatId);

        var total = await baseQuery.CountAsync(cancellationToken);

        var query = baseQuery
            .OrderByDescending(ms => ms.SentAt)
            .Include(ms => ms.Sender)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        // ToListAsync() is required to avoid the "The query has already been consumed by the provider
        var messages = await query.ToListAsync(cancellationToken);

        var pagedResponse = new PagedResult<Message>(messages, total, pageNumber, pageSize);

        return pagedResponse;
    }

    public async Task<bool> ExistsMessageAsync(Guid chatId, Guid userId, CancellationToken cancellationToken) =>
        await ValidateAsync(ms => ms.ChatId == chatId && ms.SenderId == userId, cancellationToken);
}