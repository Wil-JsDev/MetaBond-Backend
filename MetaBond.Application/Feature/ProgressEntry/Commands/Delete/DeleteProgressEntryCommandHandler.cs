using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ProgressEntry.Commands.Delete
{
    internal sealed class DeleteProgressEntryCommandHandler : ICommandHandler<DeleteProgressEntryCommand, Guid>
    {
        private readonly IProgressEntryRepository _repository;
        private readonly ILogger<DeleteProgressEntryCommandHandler> _logger;

        public DeleteProgressEntryCommandHandler(
            IProgressEntryRepository repository, 
            ILogger<DeleteProgressEntryCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(
            DeleteProgressEntryCommand request, 
            CancellationToken cancellationToken)
        {
            var progressEntry = await _repository.GetByIdAsync(request.Id);
            if (progressEntry != null)
            {
               
                await _repository.DeleteAsync(progressEntry,cancellationToken);
                _logger.LogInformation("Progress entry with ID {Id} successfully deleted.", request.Id);

                return ResultT<Guid>.Success(request.Id);
            }

            _logger.LogError("Progress entry with ID {Id} not found.", request.Id);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}
