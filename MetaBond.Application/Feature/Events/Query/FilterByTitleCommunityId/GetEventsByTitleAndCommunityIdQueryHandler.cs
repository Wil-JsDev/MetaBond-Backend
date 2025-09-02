using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;

public class GetEventsByTitleAndCommunityIdQueryHandler(
    IEventsRepository eventsRepository,
    ILogger<GetEventsByTitleAndCommunityIdQueryHandler> logger,
    IDistributedCache decoratedCache,
    ICommunitiesRepository communitiesRepository
) : IQueryHandler<GetEventsByTitleAndCommunityIdQuery, IEnumerable<EventsDto>>
{
    public async Task<ResultT<IEnumerable<EventsDto>>> Handle(
        GetEventsByTitleAndCommunityIdQuery request,
        CancellationToken cancellationToken)
    {
        var communitiesId = await EntityHelper.GetEntityByIdAsync
        (
            communitiesRepository.GetByIdAsync,
            request.CommunitiesId,
            "Communities",
            logger
        );
        if (!communitiesId.IsSuccess)
        {
            logger.LogError($"Community with ID {request.CommunitiesId} not found.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"{request.CommunitiesId} not found"));
        }

        if (string.IsNullOrEmpty(request.Title))
        {
            logger.LogError("The provided title is null or empty.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "The title cannot be null or empty"));
        }

        var exists = await eventsRepository.ValidateAsync(x => x.Title == request.Title, cancellationToken);
        if (!exists)
        {
            logger.LogError($"Event with title '{request.Title}' not found in the system.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"Not found title: {request.Title}"));
        }

        string cacheKey = $"communityId-{request.CommunitiesId}-title-{request.Title}";
        var eventsAndCommunity = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () =>
            {
                var eventsByTitle = await eventsRepository.GetEventsByTitleAndCommunityIdAsync(request.CommunitiesId,
                    request.Title,
                    cancellationToken);

                var eventsList = eventsByTitle.Select(EventsMapper.EventsToDto).ToList();

                return eventsList;
            }, cancellationToken: cancellationToken);

        var eventsDtos = eventsAndCommunity.ToList();
        if (!eventsDtos.Any())
        {
            logger.LogError(
                $"No events found for the title '{request.Title}' in the community with ID {request.CommunitiesId}.");
            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404",
                $"No events found for title '{request.Title}' in the community."));
        }

        logger.LogInformation(
            $"Successfully retrieved events for title '{request.Title}' in community with ID {request.CommunitiesId}");

        return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);
    }
}