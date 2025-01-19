using MetaBond.Application.Pagination;
using MetaBond.Domain;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IEventsRepository : IGenericRepository<Events>
    {
        Task<PagedResult<Events>> GetPagedEventsAsync(int pageNumber, int pageZize, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetFilterByTitleAsync(string title, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetOrderByIdAscAsync(Guid orderId, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> GetOrderByIdDescAsync(Guid orderId, CancellationToken cancellationToken);

        Task<IEnumerable<Events>> FilterByDateRange(DateTime dateFilter,CancellationToken cancellationToken);
    }
}
