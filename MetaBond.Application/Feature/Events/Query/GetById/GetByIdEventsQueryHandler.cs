using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetById
{
    internal sealed class GetByIdEventsQueryHandler : IQueryHandler<GetByIdEventsQuery, EventsDto>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetByIdEventsQueryHandler> _logger;

        public GetByIdEventsQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetByIdEventsQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<EventsDto>> Handle(
            GetByIdEventsQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.Id);
            if (events != null)
            {
                EventsDto eventsDto = new
                (
                    Id: events.Id,
                    Description: events.Description,
                    Title: events.Title,
                    DateAndTime: events.DateAndTime,
                    CreatedAt: events.CreateAt,
                    CommunitiesId: events.CommunitiesId,
                    ParticipationInEventId: events.ParticipationInEventId
                );

                _logger.LogInformation("Event with ID {Id} retrieved successfully.", events.Id);

                return ResultT<EventsDto>.Success(eventsDto);

            }

            _logger.LogError("Event with ID {Id} not found.", request.Id);

            return ResultT<EventsDto>.Failure(Error.NotFound("404", $"{request.Id} not found"));

        }
    }
}
