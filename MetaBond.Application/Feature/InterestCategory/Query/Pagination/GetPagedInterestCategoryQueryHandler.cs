using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.InterestCategory;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.InterestCategory.Query.Pagination;

internal sealed class GetPagedInterestCategoryQueryHandler(
    ILogger<GetPagedInterestCategoryQueryHandler> logger,
    IInterestCategoryRepository interestCategoryRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedInterestCategoryQuery, PagedResult<InterestCategoryGeneralDTos>>
{
    public async Task<ResultT<PagedResult<InterestCategoryGeneralDTos>>> Handle(GetPagedInterestCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var validationResultPagination =
            PaginationHelper.ValidatePagination<InterestCategoryGeneralDTos>(request.PageNumber, request.PageSize,
                logger);
        if (!validationResultPagination.IsSuccess)
            return validationResultPagination.Error!;


        var cacheKey = $"paged-interest-category-page-{request.PageNumber}-size-{request.PageSize}";

        var result = await cache.GetOrCreateAsync(cacheKey,
            async () =>
            {
                var pagedResult = await interestCategoryRepository.GetPagedAsync(request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var interestCategoryDto =
                    pagedResult.Items!.Select(InterestCategoryMapper.MapInterestCategoryGeneralDTos);

                PagedResult<InterestCategoryGeneralDTos> pagedResultDto = new(
                    items: interestCategoryDto,
                    currentPage: pagedResult.CurrentPage,
                    totalItems: pagedResult.TotalItems,
                    pageSize: request.PageSize
                );

                return pagedResultDto;
            },
            cancellationToken: cancellationToken);

        if (!result.Items!.Any())
        {
            logger.LogWarning("No interest categories found.");

            return ResultT<PagedResult<InterestCategoryGeneralDTos>>.Failure(
                Error.NotFound("404", "No interest categories found."));
        }

        logger.LogInformation("Interest categories were successfully retrieved.");

        return ResultT<PagedResult<InterestCategoryGeneralDTos>>.Success(result);
    }
}