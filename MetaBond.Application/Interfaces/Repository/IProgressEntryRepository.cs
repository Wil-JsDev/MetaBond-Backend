using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IProgressEntryRepository : IGenericRepository<ProgressEntry>
    {
        Task<PagedResult<ProgressEntry>> GetPagedProgressEntryAsync(int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetOrderByIdAsync(CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetEntriesByDateRangeAsync(DateTime startTime ,DateTime endTime,CancellationToken cancellationToken);

        Task<int> CountEntriesByBoardIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetRecentEntriesAsync(int topCount, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetOrderByDescriptionAsync(CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetByIdProgressEntryWithProgressBoard(Guid progressEntry, CancellationToken cancellationToken);
    }
}
