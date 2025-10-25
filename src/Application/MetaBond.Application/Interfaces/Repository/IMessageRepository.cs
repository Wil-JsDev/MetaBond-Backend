using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<PagedResult<Message>> GetPagedMessagesAsync(int pageNumber, int pageSize, Guid chatId,
        CancellationToken cancellationToken);

    Task<bool> ExistsMessageAsync(Guid chatId, Guid userId, CancellationToken cancellationToken);
}