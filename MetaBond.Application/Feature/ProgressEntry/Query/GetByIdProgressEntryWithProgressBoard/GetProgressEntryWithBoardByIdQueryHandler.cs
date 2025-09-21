using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;

internal sealed class GetProgressEntryWithBoardByIdQueryHandler(
    IProgressEntryRepository repository,
    IDistributedCache decoratedCache,
    ILogger<GetProgressEntryWithBoardByIdQueryHandler> logger)
    : IQueryHandler<GetProgressEntryWithBoardByIdQuery, PagedResult<ProgressEntryWithProgressBoardDTos>>
{
    public async Task<ResultT<PagedResult<ProgressEntryWithProgressBoardDTos>>> Handle(
        GetProgressEntryWithBoardByIdQuery request,
        CancellationToken cancellationToken)
    {
        var progressEntry = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.ProgressEntryId,
            "ProgressEntry",
            logger
        );

        if (!progressEntry.IsSuccess) return progressEntry.Error!;

        var validationPagination =
            PaginationHelper.ValidatePagination<ProgressEntryWithProgressBoardDTos>(request.PageNumber,
                request.PageSize,
                logger);

        if (!validationPagination.IsSuccess) return validationPagination.Error!;

        var progressEntryWithProgressBoard = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-with-progress-board-{request.ProgressEntryId}",
            async () =>
            {
                var idProgressEntryWithProgressBoard = await repository.GetByIdProgressEntryWithProgressBoard(
                    request.ProgressEntryId,
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = idProgressEntryWithProgressBoard.Items ?? [];

                PagedResult<ProgressEntryWithProgressBoardDTos> pagedResult = new()
                {
                    CurrentPage = idProgressEntryWithProgressBoard.CurrentPage,
                    TotalItems = idProgressEntryWithProgressBoard.TotalItems,
                    TotalPages = idProgressEntryWithProgressBoard.TotalPages,
                    Items = items.ToProgressEntryWithBoardDtos()
                };

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var itemsDto = progressEntryWithProgressBoard.Items ?? [];

        var progressEntryWithProgressBoardDTosEnumerable = itemsDto.ToList();
        if (!progressEntryWithProgressBoardDTosEnumerable.Any())
        {
            logger.LogError("No progress board entries found for ProgressEntry ID {ProgressEntryId}",
                request.ProgressEntryId);

            return ResultT<PagedResult<ProgressEntryWithProgressBoardDTos>>.Failure(Error.Failure("400",
                "No related progress board entries found."));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} progress board entries for ProgressEntry ID {ProgressEntryId}",
            progressEntryWithProgressBoardDTosEnumerable.Count(), request.ProgressEntryId);

        return ResultT<PagedResult<ProgressEntryWithProgressBoardDTos>>.Success(
            progressEntryWithProgressBoard);
    }
}