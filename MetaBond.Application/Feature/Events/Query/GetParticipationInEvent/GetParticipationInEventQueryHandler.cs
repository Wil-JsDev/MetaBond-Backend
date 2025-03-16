using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetParticipationInEvent
{
    internal sealed class GetParticipationInEventQueryHandler(
        IEventsRepository eventsRepository,
        ILogger<GetParticipationInEventQueryHandler> logger)
        : IQueryHandler<GetParticipationInEventQuery, IEnumerable<EventsWithParticipationInEventsDTos>>
    {
        public async Task<ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>> Handle(
            GetParticipationInEventQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (events != null)
            {

                IEnumerable<Domain.Models.Events> eventsWithParticipationInEvent = await eventsRepository.GetParticipationInEventAsync(events.Id,cancellationToken);
                if ( eventsWithParticipationInEvent == null || !eventsWithParticipationInEvent.Any())
                {
                    logger.LogError("No participation found for the event with ID: {EventId}", events.Id);

                    return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }
                logger.LogInformation("Found {Count} participation for the event with ID: {EventId}", eventsWithParticipationInEvent.Count(), events.Id);

                IEnumerable<EventsWithParticipationInEventsDTos> eventsWithParticipationInEventsDtos = eventsWithParticipationInEvent.Select(x => new EventsWithParticipationInEventsDTos
                (
                    EventsId: x.Id,
                    Description: x.Description,
                    Title: x.Title,
                    DateAndTime: x.DateAndTime,
                    EventParticipations: x.EventParticipations,
                    CreatedAt: x.CreateAt
                ));
                logger.LogInformation("Successfully generated the DTO response for the event with ID: {EventId}", events.Id);

                return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Success(eventsWithParticipationInEventsDtos);
            }
            logger.LogError("No event found with the provided ID: {EventId}", request.EventsId);

            return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.NotFound("404",$"{request.EventsId} not found"));
        }
    }
}
