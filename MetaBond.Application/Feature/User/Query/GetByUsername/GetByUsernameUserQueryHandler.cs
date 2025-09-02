using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
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
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            logger.LogWarning("Username is required but was null or empty.");

            return ResultT<UserDTos>.Failure(
                Error.Failure("400", "The username cannot be null or empty."));
        }

        var user = await userRepository.GetByUsernameAsync(request.Username!, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("No user found with the username '{Username}'.", request.Username);

            return ResultT<UserDTos>.Failure(
                Error.NotFound("404", $"No user was found with the username '{request.Username}'."));
        }

        var userDTos = UserMapper.MapUserDTos(user);

        logger.LogInformation("Successfully retrieved user with username '{Username}'.", request.Username);

        return ResultT<UserDTos>.Success(userDTos);
    }
}