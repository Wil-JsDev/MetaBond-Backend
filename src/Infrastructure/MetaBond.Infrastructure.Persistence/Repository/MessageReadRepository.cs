using MetaBond.Application.Interfaces.Repository;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class MessageReadRepository(MetaBondContext metaBondContext)
    : GenericRepository<MessageRead>(metaBondContext), IMessageReadRepository
{
    public async Task<MessageRead?> GetMessageReadAsync(Guid messageId, Guid userId,
        CancellationToken cancellationToken)
    {
        var messageRead = await _metaBondContext.Set<MessageRead>()
            .AsNoTracking()
            .Where(mr => mr.MessageId == messageId && mr.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        return messageRead;
    }

    public async Task<bool> ExistsMessageReadAsync(Guid messageId, Guid userId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(mr => mr.MessageId == messageId && mr.UserId == userId, cancellationToken);
    }

    public async Task CreateRangeAsync(List<MessageRead> messageRead, CancellationToken cancellationToken)
    {
        await _metaBondContext.AddRangeAsync(messageRead, cancellationToken);
        await SaveAsync(cancellationToken);
    }
}