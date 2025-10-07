using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Query.GetById;

internal sealed class GetByIdProgressEntryQueryHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<GetByIdProgressEntryQueryHandler> logger)
    : IQueryHandler<GetByIdProgressEntryQuery, ProgressEntryDTos>
{
    public async Task<ResultT<ProgressEntryDTos>> Handle(GetByIdProgressEntryQuery request,
        CancellationToken cancellationToken)
    {
        var progressEntry = await EntityHelper.GetEntityByIdAsync
        (
            progressEntryRepository.GetByIdAsync,
            request.ProgressEntryId,
            "ProgressEntry",
            logger
        );

        if (!progressEntry.IsSuccess) return progressEntry.Error!;

        var progressEntryDto = ProgressEntryMapper.ToDto(progressEntry.Value);

        logger.LogInformation("Progress entry with ID {Id} retrieved successfully.", progressEntry.Value.Id);

        return ResultT<ProgressEntryDTos>.Success(progressEntryDto);
    }
}