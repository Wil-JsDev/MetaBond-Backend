using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IPostsRepository : IGenericRepository<Posts>
    {
        Task<PagedResult<Posts>> GetPagedPostsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);
    }
}
