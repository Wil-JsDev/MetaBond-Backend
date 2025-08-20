using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
    public async Task<ResultT<ProgressEntryDTos>> Handle(GetByIdProgressEntryQuery request,
        CancellationToken cancellationToken)
    {
        var progressEntry = await progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
        if (progressEntry != null)
        {
            var progressEntryDto = ProgressEntryMapper.ToDto(progressEntry);

            logger.LogInformation("Progress entry with ID {Id} retrieved successfully.", progressEntry.Id);

            return ResultT<ProgressEntryDTos>.Success(progressEntryDto);
        }

        logger.LogError("Progress entry with ID {Id} not found.", request.ProgressEntryId);

        return ResultT<ProgressEntryDTos>.Failure(Error.NotFound("400", $"{request.ProgressEntryId} not found"));
    }
}