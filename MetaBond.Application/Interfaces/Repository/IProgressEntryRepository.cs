using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IProgressEntryRepository : IGenericRepository<ProgressEntry>
    {
        Task<PagedResult<ProgressEntry>> GetPagedProgressEntryAsync(int pageSize, int pageNumber, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetOrderByIdAsync(Guid progressBoardId,CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetEntriesByDateRangeAsync(Guid progressBoardId,DateTime startTime ,DateTime endTime,CancellationToken cancellationToken);

        Task<int> CountEntriesByBoardIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetRecentEntriesAsync(Guid progressBoardId,int topCount, CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetOrderByDescriptionAsync(Guid progressBoardId,CancellationToken cancellationToken);

        Task<IEnumerable<ProgressEntry>> GetByIdProgressEntryWithProgressBoard(Guid progressEntry, CancellationToken cancellationToken);
        
        Task<IEnumerable<ProgressEntry>> GetProgressEntriesWithAuthorsAsync (Guid progressBoardId, CancellationToken cancellationToken);
    }
}
