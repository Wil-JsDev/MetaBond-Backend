using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent
{
    internal sealed class GetEventsWithParticipationInEventQueryHandler(
        IEventsRepository eventsRepository,
        ILogger<GetEventsWithParticipationInEventQueryHandler> logger)
        : IQueryHandler<GetEventsWithParticipationInEventQuery, EventsWithParticipationInEventsDTos>
    {
        public async Task<ResultT<EventsWithParticipationInEventsDTos>> Handle(
                GetEventsWithParticipationInEventQuery request,
                CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (events != null)
            {
                Domain.Models.Events eventsWithParticipationInEvents = await eventsRepository.GetEventsWithParticipationsAsync(events.Id, cancellationToken);
                
                if (eventsWithParticipationInEvents.EventParticipations != null)
                {
                    logger.LogInformation("Event {EventId} has {Count} participations.",
                        eventsWithParticipationInEvents.Id, eventsWithParticipationInEvents.EventParticipations.Count);
                }
                else
                {
                    logger.LogError("EventParticipations is NULL for event {EventId}.", eventsWithParticipationInEvents.Id);
                }
                
                EventsWithParticipationInEventsDTos inEventsDTos = new
                (
                    EventsId: eventsWithParticipationInEvents.Id,
                    Description: eventsWithParticipationInEvents.Description,
                    Title: eventsWithParticipationInEvents.Title,
                    DateAndTime: eventsWithParticipationInEvents.DateAndTime,
                    EventParticipations: eventsWithParticipationInEvents.EventParticipations,
                    CreatedAt: eventsWithParticipationInEvents.CreateAt
                );

                logger.LogInformation("Successfully retrieved event with ID: {EventId} and its participations.", eventsWithParticipationInEvents.Id);

                return ResultT<EventsWithParticipationInEventsDTos>.Success(inEventsDTos);
            }

            logger.LogError("Event with ID: {EventId} not found.", request.EventsId);

            return ResultT<EventsWithParticipationInEventsDTos>.Failure(Error.NotFound("404", $"{request.EventsId} not found"));
        }
    }
}
