using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetEvents;

internal sealed class GetEventsQueryHandler(
    IParticipationInEventRepository participationInEventRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEventsQueryHandler> logger)
    : IQueryHandler<GetEventsQuery, IEnumerable<EventsWithParticipationInEventDTos>>
{
    public async Task<ResultT<IEnumerable<EventsWithParticipationInEventDTos>>> Handle(
        GetEventsQuery request,
        CancellationToken cancellationToken)
    {
        var participationInEventId = await EntityHelper.GetEntityByIdAsync(
            participationInEventRepository.GetByIdAsync,
            request.ParticipationInEventId ?? Guid.Empty,
            "ParticipationInEvent",
            logger
        );

        if (participationInEventId.IsSuccess)
        {
            logger.LogInformation("Participation in event found with ID: {ParticipationInEventId}",
                participationInEventId.Value.Id);

            string cacheKey = $"GetEventsQueryHandler-{request.ParticipationInEventId}";

            var result = await decoratedCache.GetOrCreateAsync(
                cacheKey,
                async () =>
                {
                    var participationInEvents = await participationInEventRepository.GetEventsAsync(
                        participationInEventId.Value.Id,
                        cancellationToken);

                    var inEvents = participationInEvents.ToList();

                    return inEvents.ToParticipationInEventDtos();
                },
                cancellationToken: cancellationToken);

            var eventsWithParticipationInEventDTosEnumerable = result.ToList();

            if (!eventsWithParticipationInEventDTosEnumerable.Any())
            {
                logger.LogError("No events found for participation in event with ID: {ParticipationInEventId}",
                    participationInEventId.Value.Id);

                return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.Failure("400",
                    "The list is empty"));
            }

            logger.LogInformation(
                "Successfully retrieved {Count} events for participation in event with ID: {ParticipationInEventId}",
                eventsWithParticipationInEventDTosEnumerable.Count, participationInEventId.Value.Id);

            return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Success(
                eventsWithParticipationInEventDTosEnumerable);
        }

        logger.LogError("No participation in event found with ID: {EventId}", request.ParticipationInEventId);

        return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.NotFound("404",
            $"{request.ParticipationInEventId} not found"));
    }
}