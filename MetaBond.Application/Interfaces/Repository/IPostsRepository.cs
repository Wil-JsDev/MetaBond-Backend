using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;


namespace MetaBond.Application.Interfaces.Repository
{
    public interface IPostsRepository : IGenericRepository<Posts>
    {
        Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> GetFilterByTitleAsync(Guid communitiesId,string title,CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> GetPostsByIdWithCommunitiesAsync(Guid id,CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> FilterTop10RecentPostsAsync(Guid communitiesId,CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> FilterRecentPostsByCountAsync(Guid communitiesId,int topCount, CancellationToken cancellationToken);

        Task<IEnumerable<Posts>> GetPostWithAuthorAsync(Guid postsId, CancellationToken cancellationToken);
    }
}
