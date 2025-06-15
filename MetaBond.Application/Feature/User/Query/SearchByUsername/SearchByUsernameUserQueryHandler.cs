using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.SearchByUsername;

internal sealed class SearchByUsernameUserQueryHandler(
    IUserRepository userRepository,
    ILogger<SearchByUsernameUserQueryHandler> logger,
    IDistributedCache decoratedCache
    ) : 
    IQueryHandler<SearchByUsernameUserQuery, IEnumerable<UserDTos>>
{
    public async Task<ResultT<IEnumerable<UserDTos>>> Handle(
        SearchByUsernameUserQuery request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            var username = await decoratedCache.GetOrCreateAsync(
                $"search-username-{request.Username}",
                async () => await userRepository.SearchUsernameAsync(request.Username!, cancellationToken), 
                cancellationToken: cancellationToken
            );

            IEnumerable<Domain.Models.User> enumerable = username.ToList();
            if (!enumerable.Any())
            {
                logger.LogWarning("User not found: {Username}", request.Username);
        
                return ResultT<IEnumerable<UserDTos>>.Failure(Error.NotFound("404", "User not found"));
            }

            IEnumerable<UserDTos> userDTo = enumerable.Select(x => new UserDTos
            (
                UserId: x.Id,
                FirstName: x.FirstName,
                LastName: x.LastName,
                Username: x.Username,
                Photo: x.Photo
            ));

            logger.LogInformation("User found successfully: {Username}", request.Username!);
    
            return ResultT<IEnumerable<UserDTos>>.Success(userDTo);
        }

        logger.LogWarning("Request object is null when trying to search for user.");

        return ResultT<IEnumerable<UserDTos>>.Failure(Error.Failure("400", "Invalid request: request cannot be null."));
    }
}