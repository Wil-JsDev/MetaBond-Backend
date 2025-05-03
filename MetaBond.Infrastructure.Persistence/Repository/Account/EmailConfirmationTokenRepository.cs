using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class EmailConfirmationTokenRepository(MetaBondContext metaBondContext) :  IEmailConfirmationTokenRepository
{
    public async Task CreateToken(EmailConfirmationToken token, CancellationToken cancellationToken)
    {
        await metaBondContext.Set<EmailConfirmationToken>().AddAsync(token, cancellationToken);
        await metaBondContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<EmailConfirmationToken?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await metaBondContext.Set<EmailConfirmationToken>().
            AsNoTracking().
            FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<EmailConfirmationToken?> FindByToken(string token, CancellationToken cancellationToken)
    {
      return await metaBondContext.Set<EmailConfirmationToken>()
           .AsNoTracking()
           .FirstOrDefaultAsync(e => e.Code == token, cancellationToken: cancellationToken);
    }
    
    public async Task DeleteToken(EmailConfirmationToken token, CancellationToken cancellationToken)
    {
        metaBondContext.Set<EmailConfirmationToken>().Remove(token);
        await metaBondContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsToken(string token, CancellationToken cancellationToken)
    {
        return await metaBondContext.Set<EmailConfirmationToken>()
            .AsNoTracking()
            .AnyAsync(e => e.Code == token, cancellationToken);
    }
}