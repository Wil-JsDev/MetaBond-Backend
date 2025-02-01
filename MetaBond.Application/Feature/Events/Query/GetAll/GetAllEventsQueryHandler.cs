using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetAll
{
    internal sealed class GetAllEventsQueryHandler : IQueryHandler<GetAllEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetAllEventsQueryHandler> _logger;

        public GetAllEventsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetAllEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(
            GetAllEventsQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetAll(cancellationToken);
            if (events != null)
            {

                if (!events.Any())
                {
                    _logger.LogError("The events list is empty.");

                    return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "The list is empty"));

                }

                IEnumerable<EventsDto> eventsDtos = events.Select(x => new EventsDto 
                (
                    Id: x.Id,
                    Description: x.Description,
                    Title: x.Title,
                    DateAndTime: x.DateAndTime,
                    CreatedAt: x.CreateAt,
                    CommunitiesId: x.CommunitiesId,
                    ParticipationInEventId: x.ParticipationInEventId
                ));

                _logger.LogInformation("Events retrieved successfully");
                
                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);

            }

            _logger.LogError("No events found. Please try again later.");

            return ResultT<IEnumerable<EventsDto>>.Failure(Error.Failure("400", "No events found. Please try again later."));

        }
    }
}
