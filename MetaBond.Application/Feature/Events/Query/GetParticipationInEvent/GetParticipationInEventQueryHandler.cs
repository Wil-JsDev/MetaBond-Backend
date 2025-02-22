using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Events.Query.GetParticipationInEvent
{
    internal sealed class GetParticipationInEventQueryHandler : IQueryHandler<GetParticipationInEventQuery, IEnumerable<EventsWithParticipationInEventsDTos>>
    {
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetParticipationInEventQueryHandler> _logger;

        public GetParticipationInEventQueryHandler(
            IEventsRepository eventsRepository, 
            ILogger<GetParticipationInEventQueryHandler> logger)
        {
            _eventsRepository = eventsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>> Handle(
            GetParticipationInEventQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (events != null)
            {

                IEnumerable<Domain.Models.Events> eventsWithParticipationInEvent = await _eventsRepository.GetParticipationInEventAsync(events.Id,cancellationToken);
                if ( eventsWithParticipationInEvent == null || !eventsWithParticipationInEvent.Any())
                {
                    _logger.LogError("No participations found for the event with ID: {EventId}", events.Id);

                    return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }
                _logger.LogInformation("Found {Count} participations for the event with ID: {EventId}", eventsWithParticipationInEvent.Count(), events.Id);

                IEnumerable<EventsWithParticipationInEventsDTos> eventsWithParticipationInEventsDtos = eventsWithParticipationInEvent.Select(x => new EventsWithParticipationInEventsDTos
                (
                    EventsId: x.Id,
                    Description: x.Description,
                    Title: x.Title,
                    DateAndTime: x.DateAndTime,
                    EventParticipations: x.EventParticipations,
                    CreatedAt: x.CreateAt
                ));
                _logger.LogInformation("Successfully generated the DTO response for the event with ID: {EventId}", events.Id);

                return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Success(eventsWithParticipationInEventsDtos);
            }
            _logger.LogError("No event found with the provided ID: {EventId}", request.EventsId);

            return ResultT<IEnumerable<EventsWithParticipationInEventsDTos>>.Failure(Error.NotFound("404",$"{request.EventsId} not found"));
        }
    }
}
