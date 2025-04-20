using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository;

public interface IUserRepository : IGenericRepository<User>
{
   Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
   
   Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken );
   
   Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
   Task<IEnumerable<User>> SearchUsernameAsync(string keyword, CancellationToken cancellationToken);
}