using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetOrderById
{
    internal sealed class GetOrderByIdEventsQueryHandler : IQueryHandler<GetOrderByIdEventsQuery, IEnumerable<EventsDto>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetOrderByIdEventsQueryHandler> _logger;

        public GetOrderByIdEventsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetOrderByIdEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsDto>>> Handle(GetOrderByIdEventsQuery request, CancellationToken cancellationToken)
        {
            if (request.Order == "asc")
            {
                var eventsAsc = await _eventsRepository.GetOrderByIdAscAsync(cancellationToken);

                IEnumerable<EventsDto> eventsDtos = eventsAsc.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                _logger.LogInformation("Events retrieved successfully in ascending order.");

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);

            }
            else if (request.Order == "desc")
            {
                var eventsAsc = await _eventsRepository.GetOrderByIdDescAsync(cancellationToken);

                IEnumerable<EventsDto> eventsDtos = eventsAsc.Select(e => new EventsDto
                (
                    Id: e.Id,
                    Description: e.Description,
                    Title: e.Title,
                    DateAndTime: e.DateAndTime,
                    CreatedAt: e.CreateAt,
                    CommunitiesId: e.CommunitiesId,
                    ParticipationInEventId: e.ParticipationInEventId
                ));

                _logger.LogInformation("Events retrieved successfully in descending order.");

                return ResultT<IEnumerable<EventsDto>>.Success(eventsDtos);

            }

            _logger.LogError("Invalid order parameter received: {Order}. Please specify 'asc' or 'desc'.", request.Order);

            return ResultT<IEnumerable<EventsDto>>.Failure
                (Error.Failure("400", "Invalid order parameter. Please specify 'asc' or 'desc'."));

        }
    }
}
