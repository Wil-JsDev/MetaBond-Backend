using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(FilterByTitleEventsQuery request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Title))
            {
                logger.LogError("The provided title is null or empty.");

                return ResultT<IEnumerable<EventsDto>>.Failure(
                    Error.Failure("400", "The title cannot be null or empty"));
            }

            var exists = await eventsRepository.ValidateAsync(x => x.Title == request.Title, cancellationToken);
            if (!exists)
            {
                logger.LogError("The specified event '{Title}' was not found.", request.Title);

                return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404",
                    $"The event '{request.Title}' does not exist."));
            }

            var result = await decoratedCache.GetOrCreateAsync(
                $"events-with-title-{request.Title}",
                async () =>
                {
                    var eventsTitle = await eventsRepository.GetFilterByTitleAsync(request.Title, cancellationToken);

                    IEnumerable<EventsDto> eventsDtos = eventsTitle.Select(EventsMapper.EventsToDto);

                    return eventsDtos;
                },
                cancellationToken: cancellationToken);

            if (!result.Any())
            {
                logger.LogError("No events found for the title '{Title}'.", request.Title);

                return ResultT<IEnumerable<EventsDto>>.Failure(Error.NotFound("404",
                    $"No events found for title '{request.Title}'"));
            }

            logger.LogInformation("Successfully retrieved events with title containing: {Title}", request.Title);

            return ResultT<IEnumerable<EventsDto>>.Success(result);
        }
    }
}