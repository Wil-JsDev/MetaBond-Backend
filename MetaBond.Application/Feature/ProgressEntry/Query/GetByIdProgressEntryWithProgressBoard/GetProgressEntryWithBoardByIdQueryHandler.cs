using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetByIdProgressEntryWithProgressBoard;

internal sealed class GetProgressEntryWithBoardByIdQueryHandler(
    IProgressEntryRepository repository,
    IDistributedCache decoratedCache,
    ILogger<GetProgressEntryWithBoardByIdQueryHandler> logger)
    : IQueryHandler<GetProgressEntryWithBoardByIdQuery, IEnumerable<ProgressEntryWithProgressBoardDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>> Handle(
        GetProgressEntryWithBoardByIdQuery request,
        CancellationToken cancellationToken)
    {
        var progressEntry = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.ProgressEntryId,
            "ProgressEntry",
            logger
        );

        if (!progressEntry.IsSuccess)
        {
            logger.LogError("Progress entry with ID {ProgressEntryId} not found.", request.ProgressEntryId);

            return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.NotFound("404",
                "Progress entry not found."));
        }

        var progressEntryWithProgressBoard = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-with-progress-board-{request.ProgressEntryId}",
            async () =>
            {
                var idProgressEntryWithProgressBoard = await repository.GetByIdProgressEntryWithProgressBoard(
                    request.ProgressEntryId,
                    cancellationToken);

                return idProgressEntryWithProgressBoard.ToProgressEntryWithBoardDtos();
            },
            cancellationToken: cancellationToken);

        var progressEntryWithProgressBoardDTosEnumerable = progressEntryWithProgressBoard.ToList();
        if (!progressEntryWithProgressBoardDTosEnumerable.Any())
        {
            logger.LogError("No progress board entries found for ProgressEntry ID {ProgressEntryId}",
                request.ProgressEntryId);

            return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Failure(Error.Failure("400",
                "No related progress board entries found."));
        }

        logger.LogInformation(
            "Successfully retrieved {Count} progress board entries for ProgressEntry ID {ProgressEntryId}",
            progressEntryWithProgressBoardDTosEnumerable.Count(), request.ProgressEntryId);

        return ResultT<IEnumerable<ProgressEntryWithProgressBoardDTos>>.Success(
            progressEntryWithProgressBoardDTosEnumerable);
    }
}