using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.FilterByTitle
{
    internal sealed class FilterByTitleEventsQueryHandler : IQueryHandler<FilterByTitleEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<FilterByTitleEventsQueryHandler> _logger;

        public FilterByTitleEventsQueryHandler(
            IEventsRepository eventsRepository,
            ILogger<FilterByTitleEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(FilterByTitleEventsQuery request, CancellationToken cancellationToken)
        {
            if (request.Title != null)
            {
                var eventsTitle = await _eventsRepository.GetFilterByTitleAsync(request.Title, cancellationToken);
                IEnumerable<EventsDto> eventsDtos = eventsTitle.Select(c => new EventsDto 
                (
                    Id: c.Id,
                    Description: c.Description,
                    Title: c.Title,
                    DateAndTime: c.DateAndTime,
                    CreatedAt: c.CreateAt,
                    CommunitiesId: c.CommunitiesId
                ));

                _logger.LogInformation("Successfully retrieved events with title containing: {Title}", request.Title);

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);

            }

            _logger.LogError("Failed to retrieve events. No events found with title containing: {Title}", request.Title);

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400",$"{request.Title} not found"));

        }
    }
}
