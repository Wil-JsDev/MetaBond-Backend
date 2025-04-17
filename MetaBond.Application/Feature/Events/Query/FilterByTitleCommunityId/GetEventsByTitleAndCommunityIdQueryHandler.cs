using System.Collections;
using System.Security.Cryptography;
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitleCommunityId;

public class GetEventsByTitleAndCommunityIdQueryHandler(
    IEventsRepository eventsRepository,
    ILogger<GetEventsByTitleAndCommunityIdQueryHandler> logger,
    IDistributedCache decoratedCache,
    ICommunitiesRepository communitiesRepository
    ) : IQueryHandler<GetEventsByTitleAndCommunityIdQuery,IEnumerable<EventsDto>>
{
    
    public async Task<ResultT<IEnumerable<EventsDto>>> Handle(
        GetEventsByTitleAndCommunityIdQuery request, 
        CancellationToken cancellationToken)
    {
        var communitiesId = await communitiesRepository.GetByIdAsync(request.CommunitiesId);
        if (communitiesId == null)
        {
            logger.LogError($"Community with ID {request.CommunitiesId} not found.");
            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"{request.CommunitiesId} not found"));
        }

        if (string.IsNullOrEmpty(request.Title))
        {
            logger.LogError("The provided title is null or empty.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "The title cannot be null or empty"));
        }
        
        var exists = await eventsRepository.ValidateAsync(x => x.Title == request.Title);
        if (!exists)
        {
            logger.LogError($"Event with title '{request.Title}' not found in the system.");
            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"Not found title: {request.Title}"));
        }
        
        string cacheKey = $"communityId-{request.CommunitiesId}-title-{request.Title}";
        var eventsAndCommunity = await decoratedCache.GetOrCreateAsync(
            cacheKey,
            async () => await eventsRepository.GetEventsByTitleAndCommunityIdAsync(request.CommunitiesId, request.Title,
                cancellationToken), cancellationToken: cancellationToken);
        
        IEnumerable<Domain.Models.Events> eventsEnumerable = eventsAndCommunity.ToList();
        if (!eventsEnumerable.Any())
        {
            logger.LogError($"No events found for the title '{request.Title}' in the community with ID {request.CommunitiesId}.");
            return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"No events found for title '{request.Title}' in the community."));
        }

        IEnumerable<EventsDto> eventsList = eventsEnumerable.Select(x => new EventsDto
        (
            Id: x.Id,
            Description: x.Description,
            Title: x.Title,
            DateAndTime: x.DateAndTime,
            CreatedAt: x.CreateAt,
            CommunitiesId: x.CommunitiesId
        ));
        
        logger.LogInformation($"Successfully retrieved events for title '{request.Title}' in community with ID {request.CommunitiesId}");
        
        return ResultT<IEnumerable<EventsDto>>.Success(eventsList);
    }
}