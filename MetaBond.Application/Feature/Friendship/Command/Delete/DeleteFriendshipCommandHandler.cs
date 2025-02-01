using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Command.Delete
{
    internal sealed class DeleteFriendshipCommandHandler : ICommandHandler<DeleteFriendshipCommand, Guid>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<DeleteFriendshipCommandHandler> _logger;

        public DeleteFriendshipCommandHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<DeleteFriendshipCommandHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(
            DeleteFriendshipCommand request, 
            CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            {
                await _friendshipRepository.DeleteAsync(friendship,cancellationToken);

                _logger.LogInformation("Friendship with ID: {FriendshipId} deleted successfully.", friendship.Id);

                return ResultT<Guid>.Success(friendship.Id);
            }

            _logger.LogError("Failed to delete friendship: ID {FriendshipId} not found.", request.Id);

            return ResultT<Guid>.Failure(Error.Failure("404", $"{request.Id} not found"));

        }
    }
}
