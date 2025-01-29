using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Friendship.Command.Delete
{
    internal sealed class DeleteFriendshipCommandHandler : ICommandHandler<DeleteFriendshipCommand, Guid>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public DeleteFriendshipCommandHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<Guid>> Handle(DeleteFriendshipCommand request, CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            {
                await _friendshipRepository.DeleteAsync(friendship,cancellationToken);
                return ResultT<Guid>.Success(friendship.Id);
            }

            return ResultT<Guid>.Failure(Error.Failure("404", $"{request.Id} not found"));
        }
    }
}
