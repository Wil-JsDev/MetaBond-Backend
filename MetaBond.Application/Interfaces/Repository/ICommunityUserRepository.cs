using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

public interface ICommunityUserRepository : IGenericRepository<CommunityUser>
{
    Task<PagedResult<CommunityUser>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
    
    Task<IEnumerable<CommunityUser>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<IEnumerable<CommunityUser>> GetByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);
    
    Task<IEnumerable<CommunityUser>> GetCommunitiesByUserIdAsync(Guid userId, CancellationToken cancellationToken);
    
    Task<bool> ExistsAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);

    Task<int> CountMembersByCommunityIdAsync(Guid communityId, CancellationToken cancellationToken);
    
    Task RemoveAsync(Guid userId, Guid communityId, CancellationToken cancellationToken);
}