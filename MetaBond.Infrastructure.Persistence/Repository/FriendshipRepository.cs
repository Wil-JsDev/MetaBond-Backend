using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain;
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
    public class FriendshipRepository : GenericRepository<Friendship>, IFriendshipRepository
    {
        public FriendshipRepository(MetaBondContext metaBondContext) : base(metaBondContext)
        {
        }

        public async Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageZize, CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Friendship>().AsNoTracking().CountAsync(cancellationToken);

            var pagedFriendship = await _metaBondContext.Set<Friendship>().AsNoTracking()
                                  .OrderBy(f => f.Id)
                                  .Skip((pageNumber - 1) * pageZize)
                                  .Take(pageZize)
                                  .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(pagedFriendship,pageNumber,pageZize, totalRecord);
            return pagedResponse;
        }
        public async Task<IEnumerable<Friendship>> GetFilterByStatusAsync(Status status, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                               .AsNoTracking()
                                               .Where(f => f.Status == status)
                                               .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Friendship>> OrderByIdAscAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                        .AsNoTracking()
                                        .OrderBy(x => x.Id)
                                        .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Friendship>> OrderByIdDescAsync(CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                              .AsNoTracking()
                                              .OrderByDescending(x => x.Id)
                                              .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<int> CountByStatusAsync(Status status, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                              .AsNoTracking()
                                              .Where(x => x.Status == status)
                                              .CountAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Friendship>> GetCreatedAfterAsync(DateTime date, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                              .AsQueryable()
                                              .Where(x => x.CreateAdt > date)
                                              .ToListAsync(cancellationToken);

            return query;
        }

        public async Task<IEnumerable<Friendship>> GetCreatedBeforeAsync(DateTime date, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                                              .AsQueryable()
                                              .Where(x => x.CreateAdt <= date)
                                              .ToListAsync(cancellationToken);
            return query;
        }

        public async Task<IEnumerable<Friendship>> GetRecentlyCreatedAsync(int limit, CancellationToken cancellationToken)
        {
            DateTime? date = new DateTime()
                                .AddDays(-3);
            var query = await _metaBondContext.Set<Friendship>()
                                              .AsNoTracking()
                                              .Where(x => x.CreateAdt < date)
                                              .Take(limit)
                                              .ToListAsync(cancellationToken);

            return query;
        }
    }
}
