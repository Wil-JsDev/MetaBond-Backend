using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Domain.Models;
using MetaBond.Infrastructure.Persistence.Context;

namespace MetaBond.Infrastructure.Persistence.Repository.Account;

public class UserInterestRepository(MetaBondContext metaBondContext)
    : GenericRepository<UserInterest>(metaBondContext), IUserInterestRepository;