using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderById;

internal sealed class GetOrderByIdProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByIdProgressEntryQueryHandler> logger)
    : IQueryHandler<GetOrderByIdProgressEntryQuery, IEnumerable<ProgressEntryBasicDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryBasicDTos>>> Handle(
        GetOrderByIdProgressEntryQuery request,
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

        var result = await decoratedCache.GetOrCreateAsync(
            $"order-by-id-progress-board-{request.ProgressBoardId}",
            async () =>
            {
                var progressEntries =
                    await progressEntryRepository.GetOrderByIdAsync(request.ProgressBoardId, cancellationToken);

                var entryBasicDtos = progressEntries.ToBasicDtos();

                return entryBasicDtos;
            },
            cancellationToken: cancellationToken);

        var enumerable = result.ToList();
        if (!enumerable.Any())
        {
            logger.LogError("No progress entries found when ordering by ID.");

            return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Failure(Error.Failure("400",
                "No progress entries available."));
        }

        logger.LogInformation("Successfully retrieved {Count} progress entries ordered by ID.",
            enumerable.Count);

        return ResultT<IEnumerable<ProgressEntryBasicDTos>>.Success(enumerable);
    }
}