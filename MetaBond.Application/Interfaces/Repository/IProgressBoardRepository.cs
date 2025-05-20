using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IProgressBoardRepository : IGenericRepository<ProgressBoard>
    {
        Task<PagedResult<ProgressBoard>> GetPagedBoardsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressBoard>> GetRecentBoardsAsync(DateTime dateTime, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressBoard>> GetBoardsByDateRangeAsync(DateTime startTime, DateTime endTime, CancellationToken cancellationToken);

        Task<int> CountBoardsAsync(CancellationToken cancellationToken);

        Task<IEnumerable<ProgressBoard>> GetBoardsWithEntriesAsync(Guid id,CancellationToken cancellationToken);

        Task<IEnumerable<ProgressBoard>> GetProgressBoardsWithAuthorAsync(Guid progressBoardId, CancellationToken cancellationToken);
    }
}
