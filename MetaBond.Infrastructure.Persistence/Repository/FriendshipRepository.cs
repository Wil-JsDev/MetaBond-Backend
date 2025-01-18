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
    }
}
