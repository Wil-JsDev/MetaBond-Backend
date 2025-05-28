using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using MetaBond.Domain;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.GetUserWithFriendship;

internal sealed class GetUserWithFriendshipByIdQueryHandler(
    IUserRepository userRepository,
    ILogger<GetUserWithFriendshipByIdQueryHandler> logger,
    IDistributedCache decoratedCache
    ) : 
    IQueryHandler<GetUserWithFriendshipByIdQuery, UserWithFriendshipDTos>
{
    public async Task<ResultT<UserWithFriendshipDTos>> Handle(
        GetUserWithFriendshipByIdQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var user = await userRepository.GetByIdAsync(request.UserId ?? Guid.Empty);
            if (user == null)
            {
                logger.LogError("User with id: {UserId} not found", request.UserId);
                
                return ResultT<UserWithFriendshipDTos>.Failure(Error.NotFound("404",""));
            }
            
            var userWithFriendship = await decoratedCache.GetOrCreateAsync($"get-user-with-friendship-{request.UserId}", 
                async () => await userRepository.GetUserWithFriendshipsAsync(request.UserId ?? Guid.Empty, cancellationToken), 
                cancellationToken: cancellationToken);

            
            var requesterFriendships = userWithFriendship!.ReceivedFriendRequests!.Select(x => new RequesterFriendshipDTos(
                FriendshipId: x.Id,
                RequesterId: x.RequesterId ??  Guid.Empty,
                Username: x.Requester!.Username ??  string.Empty,
                StatusFriendship: x.Status
            )).ToList();

            var addresseeFriendships = userWithFriendship.SentFriendRequests!.Select(x => new AddresseeFriendshipDTos(
                FriendshipId: x.Id,
                AddresseeId: x.AddresseeId ??  Guid.Empty,
                Username: x.Addressee!.Username ??  string.Empty,
                StatusFriendship: x.Status
            )).ToList();
            
            UserWithFriendshipDTos userWithFriendshipDTos = new
            (
                UserId:  userWithFriendship!.Id,
                FirstName: userWithFriendship.FirstName,
                LastName: userWithFriendship.LastName,
                Photo:  userWithFriendship.Photo,
                Requester: requesterFriendships,
                Addressee: addresseeFriendships
            );
        
            logger.LogInformation("");
            
            return ResultT<UserWithFriendshipDTos>.Success(userWithFriendshipDTos);
        }
        logger.LogWarning("");

        return ResultT<UserWithFriendshipDTos>.Failure(Error.Failure("",""));
    }
}