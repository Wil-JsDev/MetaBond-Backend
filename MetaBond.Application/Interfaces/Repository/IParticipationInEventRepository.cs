using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Interfaces.Repository
{
    public interface IParticipationInEventRepository : IGenericRepository<ParticipationInEvent>
    {
        Task<PagedResult<ParticipationInEvent>> GetPagedParticipationInEventAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<ParticipationInEvent>> GetParticipationByEventIdAsync(Guid idEvent, CancellationToken cancellationToken);
    }
}
