using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetById;

internal sealed class GetByIdProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdProgressEntryQueryHandler> logger)
    : IQueryHandler<GetByIdProgressEntryQuery, ProgressEntryDTos>
{
    public async Task<ResultT<ProgressEntryDTos>> Handle(GetByIdProgressEntryQuery request, CancellationToken cancellationToken)
    {
        var progressEntry = await decoratedCache.GetOrCreateAsync(
            $"progress-entry-get-by-id-{request.ProgressEntryId}",
            async () => await progressEntryRepository.GetByIdAsync(request.ProgressEntryId), 
            cancellationToken: cancellationToken);
        
        if (progressEntry != null)
        {
            ProgressEntryDTos entryDTos = new
            (
                ProgressEntryId: progressEntry.Id,
                ProgressBoardId: progressEntry.ProgressBoardId,
                UserId:  progressEntry.UserId,
                Description: progressEntry.Description,
                CreatedAt: progressEntry.CreatedAt,
                UpdateAt: progressEntry.UpdateAt
            );

            logger.LogInformation("Progress entry with ID {Id} retrieved successfully.", progressEntry.Id);

            return ResultT<ProgressEntryDTos>.Success(entryDTos);
        }

        logger.LogError("Progress entry with ID {Id} not found.", request.ProgressEntryId);

        return ResultT<ProgressEntryDTos>.Failure(Error.NotFound("400",$"{request.ProgressEntryId} not found"));
    }
}