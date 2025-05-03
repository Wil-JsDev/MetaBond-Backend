using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IUserRepository : IGenericRepository<User>
{
   Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
   
   Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken );
   
   Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
   Task<IEnumerable<User>> SearchUsernameAsync(string keyword, CancellationToken cancellationToken);

   Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
   
   Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
   
   Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken);

}