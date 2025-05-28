using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.GetByUsername;

internal sealed class GetByUsernameUserQueryHandler(
    IUserRepository userRepository,
    ILogger<GetByUsernameUserQueryHandler> logger,
    IDistributedCache decoratedCache
    ) 
    : IQueryHandler<GetByUsernameUserQuery, UserDTos>
{
    public async Task<ResultT<UserDTos>> Handle(
        GetByUsernameUserQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var user = await decoratedCache.GetOrCreateAsync($"get-by-username-user-{request.Username}",
                async () => await userRepository.GetByUsernameAsync(request.Username!, cancellationToken), 
                cancellationToken: cancellationToken);
    
            if (user == null)
            {
                logger.LogWarning("User not found: {Username}", request.Username);
        
                return ResultT<UserDTos>.Failure(Error.NotFound("404", $"User '{request.Username!}' was not found."));
            }

            UserDTos userDTos = new(
                UserId: user.Id,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Username: user.Username,
                Photo: user.Photo
            );
    
            logger.LogInformation("User found: {Username}", request.Username);
    
            return ResultT<UserDTos>.Success(userDTos);
        }

        logger.LogWarning("Request is null. Unable to process username search.");

        return ResultT<UserDTos>.Failure(Error.Failure("400", "Invalid request: request cannot be null."));
    }
}