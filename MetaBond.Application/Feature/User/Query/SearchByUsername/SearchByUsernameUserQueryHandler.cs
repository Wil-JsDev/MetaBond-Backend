using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Query.SearchByUsername;

internal sealed class SearchByUsernameUserQueryHandler(
    IUserRepository userRepository,
    ILogger<SearchByUsernameUserQueryHandler> logger,
    IDistributedCache decoratedCache
) :
    IQueryHandler<SearchByUsernameUserQuery, PagedResult<UserDTos>>
{
    public async Task<ResultT<PagedResult<UserDTos>>> Handle(
        SearchByUsernameUserQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            logger.LogWarning("Username is required but was null or empty.");

            return ResultT<PagedResult<UserDTos>>.Failure(
                Error.Failure("400", "The username cannot be null or empty."));
        }

        var paginationValidation =
            PaginationHelper.ValidatePagination<UserDTos>(request.PageNumber, request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error;

        var result = await decoratedCache.GetOrCreateAsync(
            $"search-username-{request.Username.ToLowerInvariant()}-page-{request.PageNumber}-size-{request.PageSize}",
            async () =>
            {
                var username = await userRepository.SearchUsernameAsync(request.Username, request.PageNumber,
                    request.PageSize, cancellationToken);

                var items = username.Items ?? [];
                var userDTo = items.Select(UserMapper.MapUserDTos);

                PagedResult<UserDTos> pagedResult = new()
                {
                    CurrentPage = username.CurrentPage,
                    Items = userDTo,
                    TotalItems = username.TotalItems,
                    TotalPages = username.TotalPages
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken
        );

        var userList = (result.Items ?? new List<UserDTos>()).ToList();
        if (!userList.Any())
        {
            logger.LogWarning("No users found for username: {Username}", request.Username);
            return ResultT<PagedResult<UserDTos>>.Failure(Error.NotFound("404", "User not found"));
        }

        logger.LogInformation("Users found: {Count} for username: {Username}", userList.Count, request.Username);

        return ResultT<PagedResult<UserDTos>>.Success(result);
    }
}