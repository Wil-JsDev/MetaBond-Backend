using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
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
            var events = await EntityHelper.GetEntityByIdAsync(eventsRepository.GetByIdAsync,
                request.Id,
                "Events",
                logger);

            if (events.IsSuccess)
            {
                await eventsRepository.DeleteAsync(events.Value, cancellationToken);

                logger.LogInformation("Event with ID {EventId} was successfully deleted.", request.Id);

                return ResultT<Guid>.Success(request.Id);
            }

            logger.LogError("Failed to delete event. Event with ID {EventId} was not found.", request.Id);

            return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }
    }
}