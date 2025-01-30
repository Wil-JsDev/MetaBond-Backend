using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Command.Update
{
    internal sealed class UpdateFrienshipCommandHandler : ICommandHandler<UpdateFriendshipCommand, FriendshipDTos>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public UpdateFrienshipCommandHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<FriendshipDTos>> Handle(UpdateFriendshipCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            { 
                friendship.Status = request.Status;
                
                await _friendshipRepository.UpdateAsync(friendship, cancellationToken);

                FriendshipDTos friendshipDTos = new
                (
                    FriendshipId: friendship.Id,
                    Status: friendship.Status,
                    CreatedAt: friendship.CreateAdt
                );

                return ResultT<FriendshipDTos>.Success(friendshipDTos);

            }

            return ResultT<FriendshipDTos>.Failure(Error.NotFound("404",$"{request.Id} not found")); 
            
        }
    }
}