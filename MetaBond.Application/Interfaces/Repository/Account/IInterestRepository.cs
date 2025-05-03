using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IInterestRepository : IGenericRepository<Interest>
{
    Task<IEnumerable<Interest>> GetInterestsByNameAsync(string interestName,CancellationToken cancellationToken);
}