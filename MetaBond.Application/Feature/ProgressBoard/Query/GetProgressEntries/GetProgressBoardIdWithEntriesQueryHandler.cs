using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressBoard;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressBoard.Query.GetProgressEntries;

internal sealed class GetProgressBoardIdWithEntriesQueryHandler(
    IProgressBoardRepository repository,
    IDistributedCache decoratedCache,
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetProgressBoardIdWithEntriesQueryHandler> logger)
    : IQueryHandler<GetProgressBoardIdWithEntriesQuery, IEnumerable<ProgressBoardWithProgressEntryDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>> Handle(
        GetProgressBoardIdWithEntriesQuery request,
        CancellationToken cancellationToken)
    {
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            logger.LogWarning("Invalid pagination parameters: Page = {Page}, PageSize = {PageSize}", request.PageNumber,
                request.PageSize);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                Error.Failure("400",
                    "Page number and page size must be greater than zero. Please provide valid pagination values."));
        }

        var progressBoard = await repository.GetByIdAsync(request.ProgressBoardId);
        if (progressBoard is null)
        {
            logger.LogError("No progress board found for ProgressBoardId: {ProgressBoardId}",
                request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.NotFound("404",
                "No progress board found for the given id"));
        }

        var progressBoardList = await repository.GetBoardsWithEntriesAsync(request.ProgressBoardId, cancellationToken);

        var progressBoards = progressBoardList.ToList();
        if (!progressBoards.Any())
        {
            logger.LogError("No progress entries found for ProgressBoardId: {ProgressBoardId}",
                request.ProgressBoardId);

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(Error.Failure("400",
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

                var progressEntries = progressEntryPaged.Items!.ToList();

                var progressBoardWithProgressEntryDs = progressBoards.Select(board =>
                    board.ToProgressBoardWithEntriesDto(progressEntries));

                return progressBoardWithProgressEntryDs;
            },
            cancellationToken: cancellationToken);

        var boardWithProgressEntryDTosEnumerable = progressBoardWithProgressEntryDto.ToList();
        if (!boardWithProgressEntryDTosEnumerable.Any())
        {
            logger.LogWarning(
                "No se encontraron registros de ProgressBoard con ProgressEntry para la consulta realizada.");

            return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Failure(
                Error.Failure("400", "No se encontraron resultados. La lista está vacía."));
        }

        logger.LogInformation("Successfully retrieved {Count} progress entries for ProgressBoardId: {ProgressBoardId}",
            boardWithProgressEntryDTosEnumerable.Count, request.ProgressBoardId);

        return ResultT<IEnumerable<ProgressBoardWithProgressEntryDTos>>.Success(
            boardWithProgressEntryDTosEnumerable);
    }
}