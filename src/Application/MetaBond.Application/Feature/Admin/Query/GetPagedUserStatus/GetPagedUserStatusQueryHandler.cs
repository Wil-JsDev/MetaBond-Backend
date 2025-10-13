using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Admin.Query.GetPagedUserStatus;

internal sealed class GetPagedUserStatusQueryHandler(
    ILogger<GetPagedUserStatusQueryHandler> logger,
    IAdminRepository adminRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedUserStatusQuery, PagedResult<UserDTos>>
{
    public async Task<ResultT<PagedResult<UserDTos>>> Handle(GetPagedUserStatusQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidation =
            PaginationHelper.ValidatePagination<UserDTos>(request.PageNumber, request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation;

        var result =
            await cache.GetOrCreateAsync(
                $"get-paged-user-status-{request.StatusAccount}-paged-{request.PageNumber}-{request.PageSize}",
                async () =>
                {
                    var users = await adminRepository.GetPagedUserStatusAsync(request.StatusAccount,
                        request.PageNumber, request.PageSize, cancellationToken);

                    var items = users.Items ?? [];

                    var pagedUserDto = items.Select(UserMapper.MapUserDTos);

                    PagedResult<UserDTos> pagedResult = new()
                    {
                        CurrentPage = users.CurrentPage,
                        Items = pagedUserDto,
                        TotalItems = users.TotalItems,
                        TotalPages = users.TotalPages
                    };

                    return pagedResult;
                },
                cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogInformation("No users found for status account '{StatusAccount}'.", request.StatusAccount);

            return ResultT<PagedResult<UserDTos>>.Failure(
                Error.Failure("400", "No users found for status account."));
        }

        logger.LogInformation("Users found for status account '{StatusAccount}'.", request.StatusAccount);

        return ResultT<PagedResult<UserDTos>>.Success(result);
    }
}