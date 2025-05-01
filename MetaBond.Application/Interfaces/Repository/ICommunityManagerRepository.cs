using System.Linq.Expressions;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

public interface ICommunityManagerRepository : IGenericRepository<CommunityManager>
{
    
    Task<PagedResult<CommunityManager>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(Expression<Func<CommunityManager, bool>> predicate, CancellationToken cancellationToken);


    Task<bool> IsUserCommunityManagerAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);

    Task<List<CommunityManager>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);

    Task<List<CommunityManager>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);

}