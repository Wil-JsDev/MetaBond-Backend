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
                Domain.Models.Friendship friendshipModel = new()
                {
                    Id = friendship.Id,
                    Status = friendship.Status,
                    CreateAt = friendship.CreateAt
                };
                await _friendshipRepository.UpdateAsync(friendshipModel,cancellationToken);

                FriendshipDTos friendshipDTos = new
                (
                    FriendshipId: friendshipModel.Id,
                    Status: friendshipModel.Status,
                    CreatedAt: friendshipModel.CreateAt
                );

                return ResultT<FriendshipDTos>.Success(friendshipDTos);

            }

            return ResultT<FriendshipDTos>.Failure(Error.NotFound("404",$"{request.Id} not found")); 
            
        }
    }
}