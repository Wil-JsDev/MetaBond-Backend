using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetOrderByDescription;

internal sealed class GetOrderByDescriptionProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IProgressBoardRepository progressBoardRepository,
    IDistributedCache decoratedCache,
    ILogger<GetOrderByDescriptionProgressEntryQueryHandler> logger)
    : IQueryHandler<GetOrderByDescriptionProgressEntryQuery, IEnumerable<ProgressEntryWithDescriptionDTos>>
{
    public async Task<ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>> Handle(
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

        var result = await decoratedCache.GetOrCreateAsync(
            $"order-by-description-progress-entry-{request.ProgressBoardId}",
            async () =>
            {
                var progressEntries = await progressEntryRepository.GetOrderByDescriptionAsync(request.ProgressBoardId,
                    cancellationToken);

                var descriptionDTos = progressEntries.Select(x =>
                    new ProgressEntryWithDescriptionDTos
                    (
                        Description: x.Description
                    ));

                return descriptionDTos;
            },
            cancellationToken: cancellationToken);

        var progressEntryWithDescriptionDTosEnumerable = result.ToList();
        if (!progressEntryWithDescriptionDTosEnumerable.Any())
        {
            logger.LogError("No progress entries found when ordering by description.");

            return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Failure(Error.Failure("400",
                "No progress entries found for the given description order."));
        }

        logger.LogInformation("Successfully retrieved {Count} progress entries ordered by description.",
            progressEntryWithDescriptionDTosEnumerable.Count());

        return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Success(
            progressEntryWithDescriptionDTosEnumerable);
    }
}