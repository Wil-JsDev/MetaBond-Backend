using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class PostsRepository(MetaBondContext metaBondContext)
        : GenericRepository<Posts>(metaBondContext), IPostsRepository
    {
        public async Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Posts>().AsNoTracking().CountAsync(cancellationToken);

            var pagedPosts = await _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);


            PagedResult<Posts> result = new(pagedPosts, pageNumber, pageSize, totalRecord);

            return result;
        }

        public async Task<PagedResult<Posts>> FilterRecentPostsByCountAsync(Guid communitiesId,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .Where(x => x.CommunitiesId == communitiesId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            PagedResult<Posts> result = new(query, pageNumber, pageSize, total);

            return result;
        }

        public async Task<PagedResult<Posts>> GetPostWithAuthorAsync(Guid postsId, int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .Where(x => x.Id == postsId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(posts => posts.CreatedBy)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            PagedResult<Posts> result = new(query, pageNumber, pageSize, total);

            return result;
        }

        public async Task<IEnumerable<Posts>> FilterTop10RecentPostsAsync(Guid communitiesId,
            CancellationToken cancellationToken)
        {
            var posts = await _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .Where(x => x.CommunitiesId == communitiesId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToListAsync(cancellationToken);

            return posts;
        }

        public async Task<PagedResult<Posts>> GetFilterByTitleAsync(Guid communitiesId, string title,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .Where(x => x.CommunitiesId == communitiesId && x.Title == title);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            PagedResult<Posts> result = new(query, pageNumber, pageSize, total);

            return result;
        }

        public async Task<PagedResult<Posts>> GetPostsByIdWithCommunitiesAsync(Guid id,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Posts>()
                .AsNoTracking()
                .Where(x => x.Id == id);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(x => x.Communities)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            PagedResult<Posts> result = new(query, pageNumber, pageSize, total);

            return result;
        }
    }
}