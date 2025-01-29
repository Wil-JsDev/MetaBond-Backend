using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;

namespace MetaBond.Application.Feature.Friendship.Query.Pagination
{
    internal sealed class GetPagedFriendshipQueryHandler : IQueryHandler<GetPagedFriendshipQuery, PagedResult<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;

        public GetPagedFriendshipQueryHandler(IFriendshipRepository friendshipRepository)
        {
            _friendshipRepository = friendshipRepository;
        }

        public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(GetPagedFriendshipQuery request, CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var pagedFriendship = await _friendshipRepository.GetPagedFriendshipAsync(request.PageNumber,request.PageSize,cancellationToken);
                var friendshipDto = pagedFriendship.Items.Select(c => new FriendshipDTos
                (
                   FriendshipId: c.Id,
                   Status: c.Status,
                   CreatedAt: c.CreateAt
                ));

                PagedResult<FriendshipDTos> result = new()
                {
                    TotalItems = pagedFriendship.TotalItems,
                    CurrentPage = pagedFriendship.CurrentPage,
                    TotalPages = pagedFriendship.TotalPages,
                    Items = friendshipDto
                };

                return ResultT<PagedResult<FriendshipDTos>>.Success(result);
            }

            return ResultT<PagedResult<FriendshipDTos>>.Failure
                (Error.Failure("400", "No friendship were found for the specified criteria."));

        }
    }
}
