using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Update;

internal sealed class UpdateProgressEntryCommandHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<UpdateProgressEntryCommandHandler> logger)
    : ICommandHandler<UpdateProgressEntryCommand, ProgressEntryDTos>
{
    public async Task<ResultT<ProgressEntryDTos>> Handle(
        UpdateProgressEntryCommand request, 
        CancellationToken cancellationToken)
    {
        var progressEntry = await progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
        if (progressEntry != null)
        {
            progressEntry.Description = request.Description;
            progressEntry.UpdateAt = DateTime.UtcNow;

            await progressEntryRepository.UpdateAsync(progressEntry,cancellationToken);
            
            logger.LogInformation("Progress entry with ID {Id} successfully updated.", request.ProgressEntryId);
            
            var progressEntryDto = ProgressEntryMapper.ToDto(progressEntry);

            logger.LogInformation("Returning updated progress entry data.");

            return ResultT<ProgressEntryDTos>.Success(progressEntryDto);
        }

        logger.LogError("Progress entry with ID {Id} not found.", request.ProgressEntryId);

        return ResultT<ProgressEntryDTos>.Failure(Error.NotFound("404", $"{request.ProgressEntryId} not found"));
    }
}