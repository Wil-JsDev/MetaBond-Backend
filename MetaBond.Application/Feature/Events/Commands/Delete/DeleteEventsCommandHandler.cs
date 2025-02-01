using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Delete
{
    internal sealed class DeleteEventsCommandHandler : ICommandHandler<DeleteEventsCommand, Guid>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<DeleteEventsCommandHandler> _logger;

        public DeleteEventsCommandHandler(
            IEventsRepository eventsRepository, 
            ILogger<DeleteEventsCommandHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(
            DeleteEventsCommand request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                await _eventsRepository.DeleteAsync(events,cancellationToken);

                _logger.LogInformation("Event with ID {EventId} was successfully deleted.", request.Id);

                return ResultT<Guid>.Success(request.Id);

            }

            _logger.LogError("Failed to delete event. Event with ID {EventId} was not found.", request.Id);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));

        }
    }
}
