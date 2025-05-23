﻿using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetRecentlyCreated;

internal sealed class GetRecentlyCreatedFriendshipQueryHandler(
    IFriendshipRepository friendshipRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRecentlyCreatedFriendshipQueryHandler> logger)
    : IQueryHandler<GetRecentlyCreatedFriendshipQuery, IEnumerable<FriendshipDTos>>
{
    public async Task<ResultT<IEnumerable<FriendshipDTos>>> Handle(
        GetRecentlyCreatedFriendshipQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            string cacheKey = $"GetRecentlyCreatedFriendship-{request.Limit}";
            var friendshipRecently = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () => await friendshipRepository.GetRecentlyCreatedAsync(request.Limit, cancellationToken), 
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.Friendship> friendships = friendshipRecently.ToList();
            if ( !friendships.Any())
            {
                logger.LogError("No recent friendships found for the given limit: {Limit}", request.Limit);

                return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "No recent friendships found."));
            }

            var friendshipDTos = friendships.Select(x => new FriendshipDTos
            (
                FriendshipId: x.Id,
                Status: x.Status,
                RequesterId: x.RequesterId,
                AddresseeId: x.AddresseeId,
                CreatedAt: x.CreateAdt
            ));

            IEnumerable<FriendshipDTos> value = friendshipDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} recent friendships with limit: {Limit}", 
                value.Count(), request.Limit);

            return ResultT<IEnumerable<FriendshipDTos>>.Success(value);
        }

        logger.LogError("Request is null. Failed to retrieve recent friendships.");

        return ResultT<IEnumerable<FriendshipDTos>>.Failure(Error.Failure("400", "Invalid request: The request cannot be null."));
    }
}