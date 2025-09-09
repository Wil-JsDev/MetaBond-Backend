using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
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
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            logger.LogWarning("Username is required but was null or empty.");

            return ResultT<IEnumerable<UserDTos>>.Failure(
                Error.Failure("400", "The username cannot be null or empty."));
        }

        var result = await decoratedCache.GetOrCreateAsync(
            $"search-username-{request.Username}",
            async () =>
            {
                var username = await userRepository.SearchUsernameAsync(request.Username!, cancellationToken);

                IEnumerable<UserDTos> userDTo = username.Select(UserMapper.MapUserDTos);

                return userDTo;
            },
            cancellationToken: cancellationToken
        );

        List<UserDTos> enumerable = result.ToList();
        if (!enumerable.Any())
        {
            logger.LogWarning("User not found: {Username}", request.Username);

            return ResultT<IEnumerable<UserDTos>>.Failure(Error.NotFound("404", "User not found"));
        }

        logger.LogInformation("User found successfully: {Username}", request.Username!);

        return ResultT<IEnumerable<UserDTos>>.Success(enumerable);
    }
}