using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.Pagination;

internal sealed class GetPagedUserQueryHandler(
    IUserRepository userRepository,
    ILogger<GetPagedUserQueryHandler> logger,
    IDistributedCache decoratedCache
    ) : 
    IQueryHandler<GetPagedUserQuery, PagedResult<UserDTos>>
{
    public async Task<ResultT<PagedResult<UserDTos>>> Handle(
        GetPagedUserQuery request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
            {
                logger.LogWarning("Invalid pagination parameters: PageNumber = {PageNumber}, PageSize = {PageSize}.", request.PageNumber, request.PageSize);
        
                return ResultT<PagedResult<UserDTos>>.Failure(Error.Failure("400", "Invalid pagination parameters. Page number and page size must be greater than zero."));
            }

            var pagedUser = await decoratedCache.GetOrCreateAsync(
                $"get-paged-user-{request.PageNumber}-size-{request.PageSize}",
                async () => await userRepository.GetPagedUsersAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken), 
                cancellationToken: cancellationToken);

            var userDto = pagedUser.Items!.Select(x => new UserDTos(
                UserId: x.Id,
                FirstName: x.FirstName,
                LastName: x.LastName,
                Username: x.Username,
                Photo: x.Photo
            ));

            IEnumerable<UserDTos> userDTosEnumerable = userDto.ToList();
            if (!userDTosEnumerable.Any())
            {
                logger.LogWarning("No users found for the requested page: PageNumber = {PageNumber}, PageSize = {PageSize}.", request.PageNumber, request.PageSize);
        
                return ResultT<PagedResult<UserDTos>>.Failure(Error.Failure("404", "No users found for the specified page."));
            }

            PagedResult<UserDTos> pagedResultUser = new()
            {
                CurrentPage = pagedUser.CurrentPage,
                Items = userDTosEnumerable,
                TotalItems = pagedUser.TotalItems,
                TotalPages = pagedUser.TotalPages
            };

            logger.LogInformation("Paged user data successfully retrieved. PageNumber = {PageNumber}, PageSize = {PageSize}.", request.PageNumber, request.PageSize);

            return ResultT<PagedResult<UserDTos>>.Success(pagedResultUser);
        }

        logger.LogWarning("The request object was null when trying to retrieve paged users.");

        return ResultT<PagedResult<UserDTos>>.Failure(Error.Failure("400", "Invalid request. The request payload cannot be null."));

    }
}