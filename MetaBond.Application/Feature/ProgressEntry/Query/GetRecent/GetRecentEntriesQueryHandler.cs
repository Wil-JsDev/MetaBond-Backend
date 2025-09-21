using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;

internal sealed class GetRecentEntriesQueryHandler(
    IProgressEntryRepository repository,
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRecentEntriesQueryHandler> logger)
    : IQueryHandler<GetRecentEntriesQuery, PagedResult<ProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryDTos>>> Handle(
        GetRecentEntriesQuery request,
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
            PaginationHelper.ValidatePagination<ProgressEntryDTos>(request.PageNumber,
                request.PageSize,
                logger);

        if (!validationPagination.IsSuccess)
            return validationPagination.Error;

        var result = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-get-recent-{request.ProgressBoardId}",
            async () =>
            {
                var progressEntries = await repository.GetRecentEntriesAsync(request.ProgressBoardId, request.PageSize,
                    request.PageNumber, cancellationToken);

                var items = progressEntries.Items ?? [];

                var entryDTos = items.Select(ProgressEntryMapper.ToDto);

                PagedResult<ProgressEntryDTos> pagedResult = new()
                {
                    CurrentPage = progressEntries.CurrentPage,
                    TotalItems = progressEntries.TotalItems,
                    TotalPages = progressEntries.TotalPages,
                    Items = entryDTos
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var progressEntryDTosEnumerable = result.Items ?? [];
        if (!progressEntryDTosEnumerable.Any())
        {
            logger.LogError("No recent progress entries found.");

            return ResultT<PagedResult<ProgressEntryDTos>>.Failure(Error.Failure("400",
                "No recent progress entries available."));
        }

        logger.LogInformation("Successfully retrieved {Count} recent progress entries.",
            progressEntryDTosEnumerable.Count());

        return ResultT<PagedResult<ProgressEntryDTos>>.Success(result);
    }
}