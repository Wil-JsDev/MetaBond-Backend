using MetaBond.Domain.Models;

namespace MetaBond.Application.Interfaces.Repository.Account;

public interface IModeratorRepository : IGenericRepository<Moderator>
{
    Task<Moderator?> GetWithUserAsync(Guid moderatorId);
    
}