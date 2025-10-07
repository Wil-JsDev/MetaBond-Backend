using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Query.GetInterestByCategory;

internal sealed class GetInterestByCategoryIdQueryHandler(
    ILogger<GetInterestByCategoryIdQueryHandler> logger,
    IInterestRepository interestRepository,
    IInterestCategoryRepository interestCategoryRepository,
    IDistributedCache cache
) : IQueryHandler<GetInterestByCategoryIdQuery, PagedResult<InterestDTos>>
{
    public async Task<ResultT<PagedResult<InterestDTos>>> Handle(
        GetInterestByCategoryIdQuery request,
        CancellationToken cancellationToken)
    {
        var validationPaginationResult =
            PaginationHelper.ValidatePagination<InterestDTos>(request.PageNumber, request.PageSize, logger);
        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult.Error!;

        if (request.InterestCategoryId is null || !request.InterestCategoryId.Any())
        {
            logger.LogWarning("Invalid InterestCategoryId provided in GetInterestByCategoryIdQuery");

            return ResultT<PagedResult<InterestDTos>>.Failure(
                Error.Conflict("409", "InterestCategoryId cannot be null or empty."));
        }

        if (!await CategoriesExistAsync(request.InterestCategoryId, cancellationToken))
        {
            logger.LogWarning("One or more Interest Categories not found.");

            return ResultT<PagedResult<InterestDTos>>.Failure(
                Error.NotFound("CategoryNotFound", "One or more Interest Categories not found."));
        }

        var cacheKey = BuildCacheKey(request);

        var result = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var pagedResult = await interestRepository.GetPagedInterestByInterestCategoryIdAsync(
                request.InterestCategoryId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            if (pagedResult.Items == null) return null;
            var dto = pagedResult.Items.Select(InterestMapper.ModelToDto).ToList();

            PagedResult<InterestDTos> result = new(
                items: dto,
                totalItems: pagedResult.TotalItems,
                currentPage: pagedResult.CurrentPage,
                pageSize: request.PageSize
            );

            return result;
        }, cancellationToken: cancellationToken);

        if (result?.Items is not null && !result.Items.Any())
        {
            logger.LogWarning(
                "No interests found for InterestCategoryIds={Ids}, PageNumber={Page}, PageSize={Size}.",
                string.Join(",", request.InterestCategoryId),
                request.PageNumber,
                request.PageSize);

            return ResultT<PagedResult<InterestDTos>>.Failure(
                Error.NotFound("InterestsNotFound", "No interests found for the specified page."));
        }

        logger.LogInformation(
            "Returning paged interests for InterestCategoryIds={Ids}: PageNumber={Page}, PageSize={Size}, TotalItems={Total}.",
            string.Join(",", request.InterestCategoryId),
            request.PageNumber,
            request.PageSize,
            result.TotalItems);

        return ResultT<PagedResult<InterestDTos>>.Success(result);
    }

    #region Private Helpers

    private async Task<bool> CategoriesExistAsync(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        var categories = await interestCategoryRepository.GetByIdsAsync(categoryIds, cancellationToken);

        return categories.Count == categoryIds.Count;
    }

    private static string BuildCacheKey(GetInterestByCategoryIdQuery request)
    {
        if (request.InterestCategoryId == null) return string.Empty;

        var joinedIds = string.Join("-", request.InterestCategoryId.OrderBy(id => id));

        return $"get-interest-by-category-{joinedIds}-p{request.PageNumber}-s{request.PageSize}";
    }

    #endregion
}