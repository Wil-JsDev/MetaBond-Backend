﻿using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface ICommunitiesRepository : IGenericRepository<Communities>
    {
        Task<PagedResult<Communities>> GetPagedCommunitiesAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

        Task<IEnumerable<Communities>> GetByFilterAsync(Func<Communities, bool> predicate, CancellationToken cancellationToken);

        Task<IEnumerable<Communities>> GetPostsAndEventsByCommunityIdAsync(Guid communitieId, CancellationToken cancellationToken);
        Task<bool> ValidateAsync(Expression<Func<Communities, bool>> predicate);
    }
}
