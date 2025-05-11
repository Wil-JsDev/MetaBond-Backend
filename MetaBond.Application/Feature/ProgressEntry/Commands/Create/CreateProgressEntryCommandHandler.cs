using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Create;

internal sealed class CreateProgressEntryCommandHandler(
    IProgressEntryRepository progressEntryRepository,
    ILogger<CreateProgressEntryCommandHandler> logger)
    : ICommandHandler<CreateProgressEntryCommand, ProgressEntryDTos>
{
    public async Task<ResultT<ProgressEntryDTos>> Handle(
        CreateProgressEntryCommand request, 
        CancellationToken cancellationToken)
    {

        if (request != null)
        {
            Domain.Models.ProgressEntry progressEntry = new()
            {
                Id = Guid.NewGuid(),
                ProgressBoardId = request.ProgressBoardId,
                UserId = request.UserId,
                Description = request.Description
            };

            await progressEntryRepository.CreateAsync(progressEntry, cancellationToken);

            logger.LogInformation("Progress entry created successfully with ID: {ProgressEntryId}", progressEntry.Id);

            ProgressEntryDTos dTos = new
            (
                ProgressEntryId: progressEntry.Id,
                ProgressBoardId: progressEntry.ProgressBoardId,
                UserId: progressEntry.UserId,
                Description: progressEntry.Description,
                CreatedAt: progressEntry.CreatedAt,
                UpdateAt: progressEntry.UpdateAt
            );

            logger.LogInformation("Mapped ProgressEntry to DTO and returning success result.");

            return ResultT<ProgressEntryDTos>.Success(dTos);
        }
        logger.LogError("Progress entry creation failed: request was null.");
        
        return ResultT<ProgressEntryDTos>.Failure(Error.Failure("400","Invalid request"));
    }
}