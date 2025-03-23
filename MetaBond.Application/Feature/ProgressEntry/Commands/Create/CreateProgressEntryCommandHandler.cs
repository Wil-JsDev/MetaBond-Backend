using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ProgressEntry;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Create
{
    internal sealed class CreateProgressEntryCommandHandler : ICommandHandler<CreateProgressEntryCommand, ProgressEntryDTos>
    {
        private readonly IProgressEntryRepository _progressEntryRepository;
        private readonly ILogger<CreateProgressEntryCommandHandler> _logger;

        public CreateProgressEntryCommandHandler(
            IProgressEntryRepository progressEntryRepository, 
            ILogger<CreateProgressEntryCommandHandler> logger)
        {
            _progressEntryRepository = progressEntryRepository;
            _logger = logger;
        }

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
                    Description = request.Description
                };

                await _progressEntryRepository.CreateAsync(progressEntry,cancellationToken);

                _logger.LogInformation("");

                ProgressEntryDTos dTos = new
                (
                    ProgressEntryId: progressEntry.Id,
                    ProgressBoardId: progressEntry.ProgressBoardId,
                    Description: progressEntry.Description,
                    CreatedAt: progressEntry.CreatedAt,
                    UpdateAt: progressEntry.UpdateAt
                );

                _logger.LogInformation("");

                return ResultT<ProgressEntryDTos>.Success(dTos);

            }

            _logger.LogError("");

            return ResultT<ProgressEntryDTos>.Failure(Error.Failure("400","Invalid request"));
        }
    }
}
