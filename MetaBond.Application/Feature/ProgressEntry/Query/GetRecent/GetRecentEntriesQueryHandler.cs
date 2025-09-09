using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetRecent;

internal sealed class GetRecentEntriesQueryHandler(
    IProgressEntryRepository repository,
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetRecentEntriesQueryHandler> logger)
    : IQueryHandler<GetRecentEntriesQuery, IEnumerable<ProgressEntryDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryDTos>>> Handle(
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

        if (request.TopCount <= 0)
        {
            logger.LogInformation("Invalid request: TopCount must be greater than zero.");

            return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400",
                "TopCount must be greater than zero."));
        }

        var result = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-get-recent-{request.ProgressBoardId}",
            async () =>
            {
                var progressEntries = await repository.GetRecentEntriesAsync(request.ProgressBoardId, request.TopCount,
                    cancellationToken);

                IEnumerable<ProgressEntryDTos> entryDTos = progressEntries.Select(ProgressEntryMapper.ToDto);

                return entryDTos;
            },
            cancellationToken: cancellationToken);

        var progressEntryDTosEnumerable = result.ToList();
        if (!progressEntryDTosEnumerable.Any())
        {
            logger.LogError("No recent progress entries found.");

            return ResultT<IEnumerable<ProgressEntryDTos>>.Failure(Error.Failure("400",
                "No recent progress entries available."));
        }

        logger.LogInformation("Successfully retrieved {Count} recent progress entries.",
            progressEntryDTosEnumerable.Count());

        return ResultT<IEnumerable<ProgressEntryDTos>>.Success(progressEntryDTosEnumerable);
    }
}