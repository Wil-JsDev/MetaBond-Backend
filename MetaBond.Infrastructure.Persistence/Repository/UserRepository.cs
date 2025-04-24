using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository;

public class UserRepository(MetaBondContext metaBondContext) : GenericRepository<User>(metaBondContext), IUserRepository
{

    public async Task<PagedResult<User>> GetPagedAsync(int pageNumber, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalRecord = await _metaBondContext.Set<User>()
            .AsNoTracking()
            .CountAsync(cancellationToken);
        
        var pagedUser  = await _metaBondContext.Set<User>()
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<User>(pagedUser,pageNumber, pageSize, totalRecord);
    }
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user =  await  _metaBondContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, 
                cancellationToken);
        
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        return  await  _metaBondContext.Set<User>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username, 
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

       var pagedResponse = new PagedResult<User>(pagedUsers, pageNumber, pageSize, totalRecord);
       return pagedResponse;
    }
    
    public async Task<IEnumerable<User>> SearchUsernameAsync(string keyword, CancellationToken cancellationToken)
    {
        return await _metaBondContext.Set<User>()
            .AsNoTracking()
            .Where(e => e.Username!.Contains(keyword))
            .ToListAsync(cancellationToken);
    }
}