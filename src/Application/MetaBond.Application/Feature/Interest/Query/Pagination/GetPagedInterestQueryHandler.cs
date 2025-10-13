using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Interest;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Interest.Query.Pagination;

internal sealed class GetPagedInterestQueryHandler(
    ILogger<GetPagedInterestQueryHandler> logger,
    IInterestRepository interestRepository,
    IDistributedCache cache
) : IQueryHandler<GetPagedInterestQuery, PagedResult<InterestDTos>>
{
    public async Task<ResultT<PagedResult<InterestDTos>>> Handle(GetPagedInterestQuery request,
        CancellationToken cancellationToken)
    {
        // Validate page number and size
        var validationPaginationResult =
            PaginationHelper.ValidatePagination<InterestDTos>(request.PageNumber, request.PageSize, logger);
        if (!validationPaginationResult.IsSuccess)
            return validationPaginationResult.Error!;

        var result = await cache.GetOrCreateAsync($"get-paged-interest-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var paged = await interestRepository.GetPagedInterestAsync(request.PageNumber, request.PageSize,
                    cancellationToken);

                if (!paged.Items!.Any())
                    return null; // Don't cache empty results

                var dtoList = paged.Items!.Select(InterestMapper.ModelToDto).ToList();

                PagedResult<InterestDTos> resultPaged = new(
                    items: dtoList,
                    totalItems: paged.TotalItems,
                    currentPage: paged.CurrentPage,
                    pageSize: request.PageSize
                );

                logger.LogInformation(
                    "Paged interests retrieved successfully: PageNumber={PageNumber}, PageSize={PageSize}, TotalItems={TotalItems}.",
                    request.PageNumber, request.PageSize, paged.TotalItems);

                return resultPaged;
            }, cancellationToken: cancellationToken);

        // Check if no items were returned
        if (!result!.Items!.Any())
        {
            logger.LogWarning("No interests found for PageNumber={PageNumber}, PageSize={PageSize}.",
                request.PageNumber, request.PageSize);

            return ResultT<PagedResult<InterestDTos>>.Failure(
                Error.NotFound("404", "No interests found for the specified page."));
        }

        return ResultT<PagedResult<InterestDTos>>.Success(result);
    }
}