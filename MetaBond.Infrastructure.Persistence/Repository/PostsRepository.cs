using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Infrastructure.Persistence.Repository
{
    public class PostsRepository : GenericRepository<Posts>, IPostsRepository
    {
        public PostsRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Posts>().AsNoTracking().CountAsync(cancellationToken);    

            var pagedPosts = await _metaBondContext.Set<Posts>()
                                                    .AsNoTracking()
                                                    .OrderBy(x => x.Id)
                                                    .Skip((pageNumber - 1) * pageSize)
                                                    .Take(pageSize)
                                                    .ToListAsync(cancellationToken);


            PagedResult<Posts> result = new (pagedPosts,pageNumber,pageSize,totalRecord);

            return result;
        }

        public async Task<IEnumerable<Posts>> FilterRecentPostsByCountAsync(int topCount, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Posts>()
                                              .AsNoTracking()
                                              .OrderByDescending(x => x.CreatedAt)
                                              .Take(topCount)
                                              .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Posts>> FilterTop10RecentPostsAsync(CancellationToken cancellationToken)
        {
            var posts = await _metaBondContext.Set<Posts>()
                                              .AsNoTracking()
                                              .OrderByDescending(x => x.CreatedAt)
                                              .Take(10)
                                              .ToListAsync(cancellationToken);

            return posts;
        }

        public async Task<IEnumerable<Posts>> GetFilterByTitleAsync(string title, CancellationToken cancellationToken)
        {
            
            var posts = await _metaBondContext.Set<Posts>()
                                               .AsNoTracking()
                                               .Where(x => x.Title == title)
                                               .ToListAsync(cancellationToken);
            return posts;
        }


        public async Task<IEnumerable<Posts>> GetPostsByIdWithCommunitiesAsync(Guid id, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Posts>()
                                              .AsNoTracking()
                                              .Where(x => x.Id == id)
                                              .Include(x => x.Communities)
                                              .AsSplitQuery()
                                              .ToListAsync(cancellationToken);
                                              
            return query;
        }
    }
}
