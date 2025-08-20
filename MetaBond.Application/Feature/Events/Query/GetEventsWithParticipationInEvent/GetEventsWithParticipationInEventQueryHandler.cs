using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
        if (events is null)
        {
            logger.LogError("Event with ID: {EventId} not found.", request.EventsId);

            return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.NotFound("404",
                $"{request.EventsId} not found"));
        }

        var eventsWithParticipationInEvents = await decoratedCache.GetOrCreateAsync(
            $"eventsId-{request.EventsId}",
            async () =>
            {
                var eventsList =
                    await eventsRepository.GetEventsWithParticipationAsync(events.Id, cancellationToken);

                var eventsEnumerable = eventsList.ToList();

                return eventsEnumerable.ToEventsWithParticipationDtos();
            },
            cancellationToken: cancellationToken);

        var withParticipationInEventsDTosEnumerable = eventsWithParticipationInEvents.ToList();
        if (!withParticipationInEventsDTosEnumerable.Any())
        {
            logger.LogWarning("No events found with participation. Returning an empty result.");

            return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(
                Error.NotFound("404", "No events with participation were found.")
            );
        }

        IEnumerable<EventsWithParticipationInEventsDTos> eventsWithParticipationInEventsDTosEnumerable =
            withParticipationInEventsDTosEnumerable.ToList();

        logger.LogInformation("Successfully retrieved {Count} participation for Event ID: {EventId}",
            eventsWithParticipationInEventsDTosEnumerable.Count(),
            eventsWithParticipationInEventsDTosEnumerable.FirstOrDefault()?.EventsId ?? Guid.Empty);

        return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Success(
            eventsWithParticipationInEventsDTosEnumerable);
    }
}