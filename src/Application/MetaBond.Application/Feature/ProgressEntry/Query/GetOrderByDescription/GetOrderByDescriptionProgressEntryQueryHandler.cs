using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderByDescription;

internal sealed class GetOrderByDescriptionProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByDescriptionProgressEntryQueryHandler> logger)
    : IQueryHandler<GetOrderByDescriptionProgressEntryQuery, PagedResult<ProgressEntryWithDescriptionDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryWithDescriptionDTos>>> Handle(
        GetOrderByDescriptionProgressEntryQuery request,
        CancellationToken cancellationToken)
    {
        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            progressBoardRepository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess)
            return progressBoard.Error!;

        var validationPagination =
            PaginationHelper.ValidatePagination<ProgressEntryWithDescriptionDTos>(request.PageNumber,
                request.PageSize,
                logger);

        if (!validationPagination.IsSuccess) return validationPagination.Error!;

        var result = await decoratedCache.GetOrCreateAsync(
            $"order-by-description-progress-entry-{request.ProgressBoardId}",
            async () =>
            {
                var progressEntries = await progressEntryRepository.GetOrderByDescriptionAsync(request.ProgressBoardId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var itemsWithDescription = progressEntries.Items ?? [];

                var descriptionDTos = itemsWithDescription.Select(x =>
                    new ProgressEntryWithDescriptionDTos
                    (
                        Description: x.Description
                    ));

                PagedResult<ProgressEntryWithDescriptionDTos> pagedResult = new()
                {
                    CurrentPage = progressEntries.CurrentPage,
                    TotalItems = progressEntries.TotalItems,
                    TotalPages = progressEntries.TotalPages,
                    Items = descriptionDTos
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var itemsDto = result.Items ?? [];

        if (!itemsDto.Any())
        {
            logger.LogError("No progress entries found when ordering by description.");

            return ResultT<PagedResult<ProgressEntryWithDescriptionDTos>>.Failure(Error.Failure("400",
                "No progress entries found for the given description order."));
        }

        logger.LogInformation("Successfully retrieved {Count} progress entries ordered by description.",
            itemsDto.Count());

        return ResultT<PagedResult<ProgressEntryWithDescriptionDTos>>.Success(result);
    }
}