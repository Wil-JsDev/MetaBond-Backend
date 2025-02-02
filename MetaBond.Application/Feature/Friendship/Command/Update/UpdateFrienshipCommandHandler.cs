using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Command.Update
{
    internal sealed class UpdateFrienshipCommandHandler : ICommandHandler<UpdateFriendshipCommand, FriendshipDTos>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<UpdateFrienshipCommandHandler> _logger;

        public UpdateFrienshipCommandHandler(
            IFriendshipRepository friendshipRepository,
            ILogger<UpdateFrienshipCommandHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<FriendshipDTos>> Handle(
            UpdateFriendshipCommand request, 
            CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            { 
                friendship.Status = request.Status;
                
                await _friendshipRepository.UpdateAsync(friendship, cancellationToken);

                _logger.LogInformation("Friendship with ID: {FriendshipId} updated successfully. New Status: {Status}",
                                        friendship.Id, friendship.Status);

                FriendshipDTos friendshipDTos = new
                (
                    FriendshipId: friendship.Id,
                    Status: friendship.Status,
                    CreatedAt: friendship.CreateAdt
                );

                return ResultT<FriendshipDTos>.Success(friendshipDTos);

            }

            _logger.LogError("Failed to update friendship: ID {FriendshipId} not found.", request.Id);

            return ResultT<FriendshipDTos>.Failure(Error.NotFound("404",$"{request.Id} not found")); 
            
        }
    }
}