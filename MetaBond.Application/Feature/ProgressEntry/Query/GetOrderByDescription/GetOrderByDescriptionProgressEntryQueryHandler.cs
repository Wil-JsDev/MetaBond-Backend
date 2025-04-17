using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
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
        var progressBoard = await progressEntryRepository.GetByIdAsync(request.ProgressBoardId);
        if (progressBoard == null)
        {
            logger.LogError($"No progress board found with id: {request.ProgressBoardId}");

            return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Failure(Error.NotFound("404","No progress board found"));
        }


        var progressEntries = await decoratedCache.GetOrCreateAsync(
            $"order-by-description-progress-entry-{request.ProgressBoardId}",
            async () => await progressEntryRepository.GetOrderByDescriptionAsync(request.ProgressBoardId,
                cancellationToken), 
            cancellationToken: cancellationToken);
        
        IEnumerable<Domain.Models.ProgressEntry> enumerable = progressEntries.ToList();
        if (!enumerable.Any())
        {
            logger.LogError("No progress entries found when ordering by description.");

            return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Failure(Error.Failure("400", "No progress entries found for the given description order."));
        }

        IEnumerable<ProgressEntryWithDescriptionDTos> descriptionDTos = enumerable.Select(x => new ProgressEntryWithDescriptionDTos
        (
            Description: x.Description
        ));

        IEnumerable<ProgressEntryWithDescriptionDTos> value = descriptionDTos.ToList();
        logger.LogInformation("Successfully retrieved {Count} progress entries ordered by description.", value.Count());

        return ResultT<IEnumerable<ProgressEntryWithDescriptionDTos>>.Success(value);
    }
}