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

        public async Task<PagedResult<Friendship>> GetPagedFriendshipAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var totalRecord = await _metaBondContext.Set<Friendship>().AsNoTracking().CountAsync(cancellationToken);

            var pagedFriendship = await _metaBondContext.Set<Friendship>().AsNoTracking()
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(pagedFriendship, pageNumber, pageSize, totalRecord);
            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> GetFilterByStatusAsync(Status status, int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .Where(f => f.Status == status);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(f => f.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> GetFriendshipWithUsersAsync(Guid friendshipId,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .Where(us => us.Id == friendshipId);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(us => us.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Include(ad => ad.Addressee)
                .Include(re => re.Requester)
                .AsSplitQuery()
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> OrderByIdAscAsync(int pageNumber, int pageSize
            , CancellationToken cancellationToken)
        {
            var total = await _metaBondContext.Set<Friendship>().AsNoTracking().CountAsync(cancellationToken);

            var query = await _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> OrderByIdDescAsync(int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var total = await _metaBondContext.Set<Friendship>().AsNoTracking().CountAsync(cancellationToken);

            var query = await _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<int> CountByStatusAsync(Status status, CancellationToken cancellationToken)
        {
            var query = await _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .Where(x => x.Status == status)
                .CountAsync(cancellationToken);

            return query;
        }

        public async Task<PagedResult<Friendship>> GetCreatedAfterAsync(DateTime date,
            int pageNumber, int pageSize,
            CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Friendship>()
                .AsQueryable()
                .Where(x => x.CreateAdt > date);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> GetCreatedBeforeAsync(DateTime date,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var baseQuery = _metaBondContext.Set<Friendship>()
                .AsQueryable()
                .Where(x => x.CreateAdt <= date);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }

        public async Task<PagedResult<Friendship>> GetRecentlyCreatedAsync(int limit,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            DateTime? date = new DateTime()
                .AddDays(-3);
            var baseQuery = _metaBondContext.Set<Friendship>()
                .AsNoTracking()
                .Where(x => x.CreateAdt < date)
                .Take(limit);

            var total = await baseQuery.CountAsync(cancellationToken);

            var query = await baseQuery
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var pagedResponse = new PagedResult<Friendship>(query, pageNumber, pageSize, total);

            return pagedResponse;
        }
    }
}