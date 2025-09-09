using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
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
        var validationPaginationResult = PaginationHelper.ValidatePagination<UserDTos>
        (
            request.PageNumber,
            request.PageSize,
            logger
        );

        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult.Error!;

        var pagedUser = await decoratedCache.GetOrCreateAsync(
            $"get-paged-user-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var paged = await userRepository.GetPagedUsersAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var userDto = paged.Items!.Select(UserMapper.MapUserDTos);

                PagedResult<UserDTos> pagedResultUser = new()
                {
                    CurrentPage = paged.CurrentPage,
                    Items = userDto,
                    TotalItems = paged.TotalItems,
                    TotalPages = paged.TotalPages
                };

                return pagedResultUser;
            },
            cancellationToken: cancellationToken);

        if (!pagedUser.Items!.Any())
        {
            logger.LogWarning(
                "No users found for the requested page: PageNumber = {PageNumber}, PageSize = {PageSize}.",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<UserDTos>>.Failure(Error.Failure("404",
                "No users found for the specified page."));
        }

        logger.LogInformation(
            "Paged user data successfully retrieved. PageNumber = {PageNumber}, PageSize = {PageSize}.",
            request.PageNumber, request.PageSize);

        return ResultT<PagedResult<UserDTos>>.Success(pagedUser);
    }
}