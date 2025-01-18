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

        public async Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken)
        {
            

            return null;
        }
    }
}
