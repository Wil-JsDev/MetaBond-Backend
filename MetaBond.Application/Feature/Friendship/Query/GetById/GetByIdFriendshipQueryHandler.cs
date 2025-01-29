using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaBond.Application.Feature.Friendship.Query.GetById
{
    internal sealed class GetByIdFriendshipQueryHandler : IQueryHandler<GetByIdFriendshipQuery, FriendshipDTos>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public GetByIdFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<FriendshipDTos>> Handle(GetByIdFriendshipQuery request, CancellationToken cancellationToken)
        {
            var friendship = await _friendshipRepository.GetByIdAsync(request.Id);
            if (friendship != null)
            {
                FriendshipDTos friendshipDTos = new
                ( 
                    FriendshipId: friendship.Id,
                    Status: friendship.Status,
                    CreatedAt: friendship.CreateAt
                );

                return ResultT<FriendshipDTos>.Success(friendshipDTos);

            }

            return ResultT<FriendshipDTos>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
