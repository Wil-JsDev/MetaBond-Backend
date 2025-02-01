using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IFriendshipRepository : IGenericRepository<Friendship>
    {
        Task<IEnumerable<Friendship>> OrderByIdAscAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Friendship>> OrderByIdDescAsync(CancellationToken cancellationToken);

        Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

        Task<IEnumerable<Friendship>> GetCreatedBeforeAsync(DateTime date, CancellationToken cancellationToken);

        Task<IEnumerable<Friendship>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken); 

        Task<int> CountByStatusAsync(Status status, CancellationToken cancellationToken);

        Task<IEnumerable<Friendship>> GetRecentlyCreatedAsync(int limit, CancellationToken cancellationToken);

        Task<IEnumerable<Friendship>> GetFilterByStatusAsync(Status status, CancellationToken cancellationToken);
    }
}
