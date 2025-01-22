using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;


namespace MetaBond.Application.Interfaces.Repository
{
    public interface IPostsRepository : IGenericRepository<Posts>
    {
        Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> GetFilterByTitleAsync(string title,CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> GetPostsByIdWithCommunitiesAsync(Guid id,CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> FilterTop10RecentPostsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> FilterRecentPostsByCountAsync(int topCount, CancellationToken cancellationToken);
    }
}
