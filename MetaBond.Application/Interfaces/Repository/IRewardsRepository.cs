using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IRewardsRepository : IGenericRepository<Rewards>
    {
        Task<PagedResult<Rewards>> GetPagedRewardsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

        Task<Rewards> GetMostRecentRewardAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Rewards>> GetRewardsByDateRangeAsync(DateTime startTime, DateTime endTime ,CancellationToken cancellationToken);

        Task<int> CountRewardsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<Rewards>> GetTopRewardsByPointsAsync(int topCount, CancellationToken cancellationToken);
    }
}
