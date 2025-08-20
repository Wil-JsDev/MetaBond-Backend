using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.GetUserWithFriendship;

internal sealed class GetUserWithFriendshipByIdQueryHandler(
    IUserRepository userRepository,
    ILogger<GetUserWithFriendshipByIdQueryHandler> logger
) :
    IQueryHandler<GetUserWithFriendshipByIdQuery, UserWithFriendshipDTos>
{
    public async Task<ResultT<UserWithFriendshipDTos>> Handle(
        GetUserWithFriendshipByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId ?? Guid.Empty);
        if (user is null)
        {
            logger.LogError("User with ID: {UserId} not found.", request.UserId);

            return ResultT<UserWithFriendshipDTos>.Failure(Error.NotFound("404", "User not found."));
        }

        var userWithFriendship = await userRepository.GetUserWithFriendshipsAsync(request.UserId ?? Guid.Empty,
            cancellationToken);

        var userWithFriendshipDtos = FriendshipMapper.MapUserWithFriendshipDTos(userWithFriendship!);

        logger.LogInformation("User with ID: {UserId} successfully retrieved along with friendships.",
            request.UserId);

        return ResultT<UserWithFriendshipDTos>.Success(userWithFriendshipDtos);
    }
}