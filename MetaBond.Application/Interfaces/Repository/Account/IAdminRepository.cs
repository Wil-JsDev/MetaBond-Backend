using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IAdminRepository : IGenericRepository<Admin>
{
    Task<PagedResult<Admin>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}