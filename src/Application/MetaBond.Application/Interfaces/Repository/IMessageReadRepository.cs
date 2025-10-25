using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

public interface IMessageReadRepository : IGenericRepository<MessageRead>
{
    Task<MessageRead?> GetMessageReadAsync(Guid messageId, Guid userId, CancellationToken cancellationToken);

    Task<bool> ExistsMessageReadAsync(Guid messageId, Guid userId, CancellationToken cancellationToken);
}