using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;

internal sealed class GetEventsQueryHandler(
    IParticipationInEventRepository participationInEventRepository,
    ILogger<GetEventsQueryHandler> logger)
    : IQueryHandler<GetEventsQuery, IEnumerable<EventsWithParticipationInEventDTos>>
{
    public async Task<ResultT<IEnumerable<EventsWithParticipationInEventDTos>>> Handle(
        GetEventsQuery request,
        CancellationToken cancellationToken)
    {
        var participationInEventId = await participationInEventRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
        if (participationInEventId != null)
        {
            logger.LogInformation("Participation in event found with ID: {ParticipationInEventId}", participationInEventId.Id);

            IEnumerable<Domain.Models.ParticipationInEvent> participationInEvents = await participationInEventRepository.GetEventsAsync(participationInEventId.Id, cancellationToken);

            IEnumerable<Domain.Models.ParticipationInEvent> inEvents = participationInEvents.ToList();
            if (participationInEvents == null || !inEvents.Any())
            {
                logger.LogError("No events found for participation in event with ID: {ParticipationInEventId}", participationInEventId.Id);

                return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            IEnumerable<EventsWithParticipationInEventDTos> participationInEventDTos = inEvents.Select(x => new EventsWithParticipationInEventDTos
            (
                ParticipationInEventId: x.Id,
                EventParticipation: x.EventParticipations
            ));

            IEnumerable<EventsWithParticipationInEventDTos> eventsWithParticipationInEventDTosEnumerable = participationInEventDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} events for participation in event with ID: {ParticipationInEventId}", eventsWithParticipationInEventDTosEnumerable.Count(), participationInEventId.Id);

            return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Success(eventsWithParticipationInEventDTosEnumerable);
        }
        logger.LogError("No participation in event found with ID: {EventId}", request.EventsId);

        return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.NotFound("404",$"{request.EventsId} not found"));
    }

}