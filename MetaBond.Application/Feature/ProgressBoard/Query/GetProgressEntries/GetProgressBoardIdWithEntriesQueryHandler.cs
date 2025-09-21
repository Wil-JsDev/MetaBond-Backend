using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;

internal sealed class GetProgressBoardIdWithEntriesQueryHandler(
    IProgressBoardRepository repository,
    IDistributedCache decoratedCache,
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetProgressBoardIdWithEntriesQueryHandler> logger)
    : IQueryHandler<GetProgressBoardIdWithEntriesQuery, PagedResult<ProgressBoardWithProgressEntryDTos>>
{
    public async Task<ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>> Handle(
        GetProgressBoardIdWithEntriesQuery request,
        CancellationToken cancellationToken)
    {
        var paginationValidationResult = PaginationHelper.ValidatePagination<ProgressBoardWithProgressEntryDTos>(
            request.PageNumber,
            request.PageSize,
            logger
        );

        if (!paginationValidationResult.IsSuccess)
            return paginationValidationResult.Error!;

        var progressBoard = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.ProgressBoardId,
            "ProgressBoard",
            logger
        );

        if (!progressBoard.IsSuccess)
            return progressBoard.Error!;

        var validationPagination = PaginationHelper.ValidatePagination<ProgressBoardWithProgressEntryDTos>(
            request.PageNumber,
            request.PageSize,
            logger
        );

        if (!validationPagination.IsSuccess) return validationPagination.Error!;

        var progressBoardList = await repository.GetBoardsWithEntriesAsync(request.ProgressBoardId, request.PageNumber,
            request.PageSize, cancellationToken);

        var progressBoards = progressBoardList.Items ?? [];
        if (!progressBoards.Any())
        {
            logger.LogError("No progress entries found for ProgressBoardId: {ProgressBoardId}",
                request.ProgressBoardId);

            return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Failure(Error.Failure("400",
                "The list is empty"));
        }

        var progressBoardWithProgressEntryDto = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-paged-with-board-{request.PageNumber}-{request.PageSize}",
            async () =>
            {
                var progressEntryPaged = await progressEntryRepository.GetPagedProgressEntryAsync(
                    request.PageNumber,
                    request.PageSize,
                    cancellationToken);

                var items = progressEntryPaged.Items ?? [];

                var progressBoardWithProgressEntryDs = progressBoards.Select(board =>
                    board.ToProgressBoardWithEntriesDto(items));

                PagedResult<ProgressBoardWithProgressEntryDTos> pagedResult = new(
                    totalItems: progressEntryPaged.TotalItems,
                    items: progressBoardWithProgressEntryDs,
                    currentPage: progressEntryPaged.CurrentPage,
                    pageSize: request.PageSize
                );

                return pagedResult;
            },
            cancellationToken: cancellationToken);

        var items = progressBoardWithProgressEntryDto.Items ?? [];

        if (!items.Any())
        {
            logger.LogWarning(
                "No se encontraron registros de ProgressBoard con ProgressEntry para la consulta realizada.");

            return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Failure(
                Error.Failure("400", "No se encontraron resultados. La lista está vacía."));
        }

        logger.LogInformation("Successfully retrieved {Count} progress entries for ProgressBoardId: {ProgressBoardId}",
            items, request.ProgressBoardId);

        return ResultT<PagedResult<ProgressBoardWithProgressEntryDTos>>.Success(progressBoardWithProgressEntryDto);
    }
}