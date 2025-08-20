using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Feature.Events.Query.GetCommunitiesAndParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetCommunities;

internal sealed class GetEventsDetailsQueryHandler(
    IEventsRepository eventsRepository,
    IDistributedCache decoratedCache,
    ILogger<GetEventsDetailsQueryHandler> logger)
    : IQueryHandler<GetEventsDetailsQuery, IEnumerable<CommunitiesEventsDTos>>
{
    public async Task<ResultT<IEnumerable<CommunitiesEventsDTos>>> Handle(
        GetEventsDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var events = await eventsRepository.GetByIdAsync(request.Id);
        if (events is null)
        {
            logger.LogError("Event with ID {Id} not found.", request.Id);

            return ResultT<IEnumerable<CommunitiesEventsDTos>>.Failure(Error.NotFound("404", $"{request.Id} not found"));
        }

        var result = await decoratedCache.GetOrCreateAsync(
            $"events-{request.Id}",
            async () =>
            {
                var communitiesEvents = await eventsRepository.GetCommunities(request.Id, cancellationToken);

                IEnumerable<CommunitiesEventsDTos> inEventDTos = communitiesEvents.ToCommunitiesDtos();

                return inEventDTos;
            },
            cancellationToken: cancellationToken
        );

        var eventsEnumerable = result.ToList();
        if (!eventsEnumerable.Any())
        {
            logger.LogError("No communities or participation in event found for event with ID {Id}.", request.Id);

            return ResultT<IEnumerable<DTOs.Events.CommunitiesEventsDTos>>.Failure(Error.NotFound("404",
                "No communities or participation in event found for this events."));
        }

        logger.LogInformation("Event details with ID {Id} retrieved successfully.", request.Id);

        return ResultT<IEnumerable<DTOs.Events.CommunitiesEventsDTos>>.Success(eventsEnumerable);
    }
}