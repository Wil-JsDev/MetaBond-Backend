using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class UserRepository(MetaBondContext metaBondContext) : GenericRepository<User>(metaBondContext), IUserRepository
{
    
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us =>  us.Email == email, cancellationToken);
    }

    public  async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us => us.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return  await  _metaBondContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, 
                cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return  await  _metaBondContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username, 
                cancellationToken);
    }

    public async Task<bool> IsEmailConfirmedAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await ValidateAsync(us => us.Id == userId && us.IsEmailConfirmed == true, 
            cancellationToken);
            
    }

    public async Task<PagedResult<User>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
       var totalRecord = await _metaBondContext.Set<User>().AsNoTracking().CountAsync(cancellationToken);
       
       var pagedUsers = await _metaBondContext.Set<User>()
           .AsNoTracking()
           .OrderBy(u => u.Id)
           .Skip((pageNumber - 1) * pageSize)
           .Take(pageSize)
           .ToListAsync(cancellationToken);

       return new PagedResult<User>(pagedUsers, pageNumber, pageSize, totalRecord);
    }
    
    public async Task<IEnumerable<User>> SearchUsernameAsync(string keyword, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<User>()
            .AsNoTracking()
            .Where(e => e.Username!.Contains(keyword))
            .ToListAsync(cancellationToken);
    }
    public async Task<User?> GetUserWithFriendshipsAsync(Guid userId, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<User>()
            .AsNoTracking()
            .Include(ep => ep.ReceivedFriendRequests)!
                .ThenInclude(ef => ef.Requester)
            .Include(ep => ep.ReceivedFriendRequests)!
                .ThenInclude(ef => ef.Requester)
            .FirstOrDefaultAsync(us =>  us.Id == userId, cancellationToken);
    }
}