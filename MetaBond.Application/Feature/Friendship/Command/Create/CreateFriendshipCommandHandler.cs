using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Command.Create
{
    internal sealed class CreateFriendshipCommandHandler : ICommandHandler<CreateFriendshipCommand, FriendshipDTos>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public CreateFriendshipCommandHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<FriendshipDTos>> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
        {
            Domain.Models.Friendship friendship = new()
            {
                Id = Guid.NewGuid(),
                Status = request.Status
            };
            if (friendship != null)
            {
                await _friendshipRepository.CreateAsync(friendship,cancellationToken);
                FriendshipDTos friendshipDTos = new
                ( 
                    FriendshipId: friendship.Id,
                    Status: friendship.Status,
                    CreatedAt: friendship.CreateAt
                );
                return ResultT<FriendshipDTos>.Success(friendshipDTos);
            }
            return ResultT<FriendshipDTos>.Failure(Error.Failure("400", "Failed to create the friendship"));
        }
    }
}
