using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestsByUser;

internal sealed class GetInterestByUserQueryHandler(
    ILogger<GetInterestByUserQueryHandler> logger,
    IUserRepository userRepository,
    IInterestRepository interestRepository,
    IDistributedCache cache
) : IQueryHandler<GetInterestByUserQuery, PagedResult<InterestDTos>>
{
    public async Task<ResultT<PagedResult<InterestDTos>>> Handle(GetInterestByUserQuery request,
        CancellationToken cancellationToken)
    {
        // Validate pagination parameters
        var validationPaginationResult =
            PaginationHelper.ValidatePagination<InterestDTos>(request.PageNumber, request.PageSize, logger);
        if (validationPaginationResult is not null)
            return validationPaginationResult.Value;

        var user = await EntityHelper.GetEntityByIdAsync(userRepository.GetByIdAsync, request.UserId, "User", logger);
        if (!user.IsSuccess)
            return ResultT<PagedResult<InterestDTos>>.Failure(user.Error!);

        var resultCache = await cache.GetOrCreateAsync(
            $"get-interest-by-user-{request.UserId}-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var paged = await interestRepository.GetInterestsByUserAsync(
                    request.UserId, request.PageNumber, request.PageSize, cancellationToken);

                if (!paged.Items!.Any())
                {
                    logger.LogWarning(
                        "No interests found for UserId={UserId}, PageNumber={PageNumber}, PageSize={PageSize}.",
                        request.UserId, request.PageNumber, request.PageSize);
                    return null; 
                }

                var dtosList = paged.Items.Select(InterestMapper.ModelToDto).ToList();

                PagedResult<InterestDTos> pagedResult = new(
                    items: dtosList,
                    totalItems: paged.TotalItems,
                    currentPage: paged.CurrentPage,
                    pageSize: request.PageSize
                );

                logger.LogInformation(
                    "Paged interests for UserId={UserId} retrieved successfully and cached: PageNumber={PageNumber}, PageSize={PageSize}, TotalItems={TotalItems}.",
                    request.UserId, request.PageNumber, request.PageSize, paged.TotalItems);

                return pagedResult;
            }, cancellationToken: cancellationToken);

        if (resultCache is null || !resultCache.Items!.Any())
        {
            logger.LogWarning("No interests found for UserId={UserId}, PageNumber={PageNumber}, PageSize={PageSize}.",
                request.UserId, request.PageNumber, request.PageSize);

            return ResultT<PagedResult<InterestDTos>>.Failure(
                Error.NotFound("404", $"No interests found for UserId {request.UserId} on the specified page."));
        }

        logger.LogInformation(
            "Returning paged interests for UserId={UserId}: PageNumber={PageNumber}, PageSize={PageSize}, TotalItems={TotalItems}.",
            request.UserId, request.PageNumber, request.PageSize, resultCache.TotalItems);

        return ResultT<PagedResult<InterestDTos>>.Success(resultCache);
    }
}