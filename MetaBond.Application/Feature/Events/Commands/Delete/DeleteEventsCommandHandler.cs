using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Commands.Delete
{
    internal sealed class DeleteEventsCommandHandler(
        IEventsRepository eventsRepository,
        ILogger<DeleteEventsCommandHandler> logger)
        : ICommandHandler<DeleteEventsCommand, Guid>
    {
        public async Task<ResultT<Guid>> Handle(
            DeleteEventsCommand request,
            CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                await eventsRepository.DeleteAsync(events, cancellationToken);

                logger.LogInformation("Event with ID {EventId} was successfully deleted.", request.Id);

                return ResultT<Guid>.Success(request.Id);
            }

            logger.LogError("Failed to delete event. Event with ID {EventId} was not found.", request.Id);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}