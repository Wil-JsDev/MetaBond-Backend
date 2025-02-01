using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Friendship;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Friendship.Query.Pagination
{
    internal sealed class GetPagedFriendshipQueryHandler : IQueryHandler<GetPagedFriendshipQuery, PagedResult<FriendshipDTos>>
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ILogger<GetPagedFriendshipQueryHandler> _logger;

        public GetPagedFriendshipQueryHandler(
            IFriendshipRepository friendshipRepository, 
            ILogger<GetPagedFriendshipQueryHandler> logger)
        {
            _friendshipRepository = friendshipRepository;
            _logger = logger;
        }

        public async Task<ResultT<PagedResult<FriendshipDTos>>> Handle(
            GetPagedFriendshipQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var pagedFriendship = await _friendshipRepository.GetPagedFriendshipAsync(request.PageNumber,request.PageSize,cancellationToken);
                var friendshipDto = pagedFriendship.Items.Select(c => new FriendshipDTos
                (
                   FriendshipId: c.Id,
                   Status: c.Status,
                   CreatedAt: c.CreateAdt
                ));

                if (!friendshipDto.Any())
                {
                    _logger.LogError("No friendships found for page {PageNumber} with page size {PageSize}.", 
                                      request.PageNumber, request.PageSize);

                    return ResultT<PagedResult<FriendshipDTos>>.Failure(Error.Failure("400", "No friendships were found for the specified criteria."));
                }

                PagedResult<FriendshipDTos> result = new()
                {
                    TotalItems = pagedFriendship.TotalItems,
                    CurrentPage = pagedFriendship.CurrentPage,
                    TotalPages = pagedFriendship.TotalPages,
                    Items = friendshipDto
                };

                _logger.LogInformation("Successfully retrieved page {PageNumber} of friendships with page size {PageSize}. Total items: {TotalItems}, Total pages: {TotalPages}",
                                        request.PageNumber, request.PageSize, pagedFriendship.TotalItems, pagedFriendship.TotalPages);

                return ResultT<PagedResult<FriendshipDTos>>.Success(result);
            }

            _logger.LogError("Invalid request: Request parameters are null or incorrect");

            return ResultT<PagedResult<FriendshipDTos>>.Failure
                (Error.Failure("400", "No friendship were found for the specified criteria."));

        }
    }
}
