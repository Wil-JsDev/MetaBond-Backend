using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.GetById
{
    internal sealed class GetByIdFriendshipQueryHandler : IQueryHandler<GetByIdFriendshipQuery, FriendshipDTos>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetByIdFriendshipQueryHandler> _logger;

        public GetByIdFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository,
            ILogger<GetByIdFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<FriendshipDTos>> Handle(
            GetByIdFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            {
                FriendshipDTos friendshipDTos = new
                ( 
                    FriendshipId: friendship.Id,
                    Status: friendship.Status,
                    CreatedAt: friendship.CreateAdt
                );

                _logger.LogInformation("Friendship retrieved successfully. ID: {FriendshipId}, Status: {Status}",
                                       friendship.Id, friendship.Status);

                return ResultT<FriendshipDTos>.Success(friendshipDTos);

            }
            _logger.LogError("Failed to retrieve friendship: ID {FriendshipId} not found.", request.Id);

            return ResultT<FriendshipDTos>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
