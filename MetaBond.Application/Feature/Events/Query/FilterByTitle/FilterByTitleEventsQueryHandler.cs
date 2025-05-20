using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitle
{
    internal sealed class FilterByTitleEventsQueryHandler(
        IEventsRepository eventsRepository,
        IDistributedCache decoratedCache,
        ILogger<FilterByTitleEventsQueryHandler> logger)
        : IQueryHandler<FilterByTitleEventsQuery, IEnumerable<EventsDto>>
    {
        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(FilterByTitleEventsQuery request, CancellationToken cancellationToken)
        {
            if (request.Title != null)
            {
                var exists = await eventsRepository.ValidateAsync(x => x.Title == request.Title, cancellationToken);
                if (!exists)
                {
                    logger.LogError("The specified event '{Title}' was not found.", request.Title);
                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404", $"The event '{request.Title}' does not exist."));
                }
                
                var eventsTitle = await decoratedCache.GetOrCreateAsync(
                    $"events-with-title-{request.Title}", 
                    async () => await eventsRepository.GetFilterByTitleAsync(request.Title, cancellationToken), 
                    cancellationToken: cancellationToken);
                
                IEnumerable<EventsDto> eventsDtos = eventsTitle.Select(c => new EventsDto 
                (
                    Id: c.Id,
                    Description: c.Description,
                    Title: c.Title,
                    DateAndTime: c.DateAndTime,
                    CreatedAt: c.CreateAt,
                    CommunitiesId: c.CommunitiesId
                ));

                logger.LogInformation("Successfully retrieved events with title containing: {Title}", request.Title);

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);

            }

            logger.LogError("Failed to retrieve events. No events found with title containing: {Title}", request.Title);

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400",$"{request.Title} not found"));
        }
    }
}
