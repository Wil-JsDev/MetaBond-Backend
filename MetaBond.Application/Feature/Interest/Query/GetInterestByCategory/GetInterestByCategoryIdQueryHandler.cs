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
    public async Task<ResultT<PagedResult<InterestDTos>>> Handle(GetInterestByCategoryIdQuery request,
        CancellationToken cancellationToken)
    {
        var interestCategory = await EntityHelper.GetEntityByIdAsync(
            interestCategoryRepository.GetByIdAsync,
            request.InterestCategoryId ?? Guid.Empty,
            "Interest Category",
            logger);

        if (!interestCategory.IsSuccess) return ResultT<PagedResult<InterestDTos>>.Failure(interestCategory.Error!);

        var cacheKey = $"get-interest-by-category-{request.InterestCategoryId}-{request.PageNumber}-{request.PageSize}";

        var result = await cache.GetOrCreateAsync(cacheKey, async () =>
        {
            var pagedResult = await interestRepository.GetPagedInterestByInterestCategoryIdAsync(
                request.InterestCategoryId ?? Guid.Empty,
                request.PageNumber, request.PageSize, cancellationToken);

            var pagedResultDto = pagedResult.Items!.Select(InterestMapper.ModelToDto);

            PagedResult<InterestDTos> resultPaged = new(
                items: pagedResultDto,
                totalItems: pagedResult.TotalItems,
                currentPage: pagedResult.CurrentPage,
                pageSize: request.PageSize
            );

            return resultPaged;
        }, cancellationToken: cancellationToken);

        if (!result.Items!.Any())
        {
            logger.LogWarning(
                "No interests found for InterestCategoryId={InterestCategoryId}, PageNumber={PageNumber}, PageSize={PageSize}.",
                request.InterestCategoryId, request.PageNumber, request.PageSize);

            return ResultT<PagedResult<InterestDTos>>.Failure(Error.NotFound("404",
                "No interests found for the specified page."));
        }

        logger.LogInformation(
            "Returning paged interests for InterestCategoryId={InterestCategoryId}: PageNumber={PageNumber}, PageSize={PageSize}, TotalItems={TotalItems}.",
            request.InterestCategoryId, request.PageNumber, request.PageSize, result.TotalItems);

        return ResultT<PagedResult<InterestDTos>>.Success(result);
    }
}