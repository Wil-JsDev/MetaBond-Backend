using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Update
{
    internal sealed class UpdateProgressEntryCommandHandler : ICommandHandler<UpdateProgressEntryCommand, ProgressEntryDTos>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<UpdateProgressEntryCommandHandler> _logger;

        public UpdateProgressEntryCommandHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<UpdateProgressEntryCommandHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

        public async Task<ResultT<ProgressEntryDTos>> Handle(
            UpdateProgressEntryCommand request, 
            CancellationToken cancellationToken)
        {
            var progressEntry = await _progressEntryRepository.GetByIdAsync(request.ProgressEntryId);
            if (progressEntry != null)
            {
                progressEntry.Description = request.Description;
                progressEntry.UpdateAt = DateTime.UtcNow;

                await _progressEntryRepository.UpdateAsync(progressEntry,cancellationToken);
                _logger.LogInformation("Progress entry with ID {Id} successfully updated.", request.ProgressEntryId);


                ProgressEntryDTos entryDTos = new
                (
                    ProgressEntryId: progressEntry.Id,
                    ProgressBoardId: progressEntry.ProgressBoardId,
                    Description: progressEntry.Description,
                    CreatedAt: progressEntry.CreatedAt,
                    UpdateAt: progressEntry.UpdateAt
                );

                _logger.LogInformation("Returning updated progress entry data.");

                return ResultT<ProgressEntryDTos>.Success(entryDTos);
            }

            _logger.LogError("Progress entry with ID {Id} not found.", request.ProgressEntryId);

            return ResultT<ProgressEntryDTos>.Failure(Error.NotFound("404", $"{request.ProgressEntryId} not found"));
        }
    }
}
