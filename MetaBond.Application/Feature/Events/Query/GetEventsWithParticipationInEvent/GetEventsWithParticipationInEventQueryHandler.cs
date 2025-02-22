using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetEventsWithParticipationInEvent
{
    internal sealed class GetEventsWithParticipationInEventQueryHandler : IQueryHandler<GetEventsWithParticipationInEventQuery, EventsWithParticipationInEventsDTos>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetEventsWithParticipationInEventQueryHandler> _logger;

        public GetEventsWithParticipationInEventQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetEventsWithParticipationInEventQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<EventsWithParticipationInEventsDTos>> Handle(
                GetEventsWithParticipationInEventQuery request,
                CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (events != null)
            {
                Domain.Models.Events eventsWithParticipationInEvents = await _eventsRepository.GetEventsWithParticipationsAsync(events.Id, cancellationToken);

                EventsWithParticipationInEventsDTos inEventsDTos = new
                (
                    EventsId: eventsWithParticipationInEvents.Id,
                    Description: eventsWithParticipationInEvents.Description,
                    Title: eventsWithParticipationInEvents.Title,
                    DateAndTime: eventsWithParticipationInEvents.DateAndTime,
                    EventParticipations: eventsWithParticipationInEvents.EventParticipations,
                    CreatedAt: eventsWithParticipationInEvents.CreateAt
                );

                _logger.LogInformation("Successfully retrieved event with ID: {EventId} and its participations.", eventsWithParticipationInEvents.Id);

                return ResultT<EventsWithParticipationInEventsDTos>.Success(inEventsDTos);
            }

            _logger.LogError("Event with ID: {EventId} not found.", request.EventsId);

            return ResultT<EventsWithParticipationInEventsDTos>.Failure(Error.NotFound("404", $"{request.EventsId} not found"));
        }
    }
}
