using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IFriendshipRepository : IGenericRepository<Friendship>
    {
        Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);
    }
}
