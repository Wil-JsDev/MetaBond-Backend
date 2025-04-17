using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent;

    internal sealed class GetEventsWithParticipationInEventQueryHandler(
        IEventsRepository eventsRepository,
        IDistributedCache decoratedCache,
        ILogger<GetEventsWithParticipationInEventQueryHandler> logger)
        : IQueryHandler<GetEventsWithParticipationInEventQuery, IEnumerable<EventsWithParticipationInEventsDTos>>
    {
        public async Task<ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>> Handle(
                GetEventsWithParticipationInEventQuery request,
                CancellationToken cancellationToken)
        {
            var events = await eventsRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (events != null)
            {

                var eventsWithParticipationInEvents = await decoratedCache.GetOrCreateAsync(
                    $"eventsId-{request.EventsId}",
                    async () => await eventsRepository.GetEventsWithParticipationAsync(events.Id, cancellationToken), 
                    cancellationToken: cancellationToken);
                
                IEnumerable<Domain.Models.Events> withParticipationInEvents = eventsWithParticipationInEvents.ToList();
               
                if (!withParticipationInEvents.Any())
                {
                    logger.LogWarning("No events found with participation. Returning an empty result.");

                    return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(
                        Error.NotFound("404", "No events with participation were found.")
                    );
                }
                
                var inEventsDTos = withParticipationInEvents.Select(x => new EventParticipation()
                {
                    ParticipationInEventId = x.EventParticipations!.Select(eventParticipation => eventParticipation.ParticipationInEventId).FirstOrDefault(),
                    EventId = x.EventParticipations!.Select(eventParticipation => eventParticipation.EventId).FirstOrDefault(),
                });
                
                IEnumerable<EventsWithParticipationInEventsDTos> participationInEventsDTosEnumerable =
                    withParticipationInEvents.Select(x =>
                        new EventsWithParticipationInEventsDTos(
                            EventsId: x.Id,
                            Description: x.Description,
                            Title: x.Title,
                            DateAndTime: x.DateAndTime,
                            ParticipationInEvents: inEventsDTos.Select( ep => new ParticipationInEventBasicDTos
                            (
                                ParticipationInEventId:  ep.ParticipationInEventId,
                                EventId: ep.EventId
                            )),
                            CreatedAt: x.CreateAt
                        ));

                IEnumerable<EventsWithParticipationInEventsDTos> eventsWithParticipationInEventsDTosEnumerable = participationInEventsDTosEnumerable.ToList();
                logger.LogInformation("Successfully retrieved {Count} participation for Event ID: {EventId}", 
                    eventsWithParticipationInEventsDTosEnumerable.Count(), 
                    eventsWithParticipationInEventsDTosEnumerable.FirstOrDefault()?.EventsId ?? Guid.Empty);

                return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Success(eventsWithParticipationInEventsDTosEnumerable);
            }

            logger.LogError("Event with ID: {EventId} not found.", request.EventsId);

            return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.NotFound("404", $"{request.EventsId} not found"));
        }
    }
