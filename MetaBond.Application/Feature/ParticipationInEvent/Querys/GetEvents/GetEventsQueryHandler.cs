using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.GetEvents
{
    internal sealed class GetEventsQueryHandler : IQueryHandler<GetEventsQuery, IEnumerable<EventsWithParticipationInEventDTos>>
    {
        private readonly IParticipationInEventRepository _participationInEventRepository;
        private readonly ILogger<GetEventsQueryHandler> _logger;

        public GetEventsQueryHandler(
            IParticipationInEventRepository participationInEventRepository, 
            ILogger<GetEventsQueryHandler> logger)
        {
            _participationInEventRepository = participationInEventRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<EventsWithParticipationInEventDTos>>> Handle(
                 GetEventsQuery request,
                 CancellationToken cancellationToken)
        {
            var participationInEvent = await _participationInEventRepository.GetByIdAsync(request.EventsId ?? Guid.Empty);
            if (participationInEvent != null)
            {
                _logger.LogInformation("Participation in event found with ID: {ParticipationInEventId}", participationInEvent.Id);

                IEnumerable<Domain.Models.ParticipationInEvent> participationInEvents = await _participationInEventRepository.GetEventsAsync(participationInEvent.Id, cancellationToken);

                if (participationInEvents == null || !participationInEvents.Any())
                {
                    _logger.LogError("No events found for participation in event with ID: {ParticipationInEventId}", participationInEvent.Id);

                    return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<EventsWithParticipationInEventDTos> participationInEventDTos = participationInEvents.Select(x => new EventsWithParticipationInEventDTos
                (
                    ParticipationInEventId: x.Id,
                    EventParticipation: x.EventParticipations
                ));

                _logger.LogInformation("Successfully retrieved {Count} events for participation in event with ID: {ParticipationInEventId}", participationInEventDTos.Count(), participationInEvent.Id);

                return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Success(participationInEventDTos);
            }

            _logger.LogError("No participation in event found with ID: {EventId}", request.EventsId);

            return ResultT<IEnumerable<EventsWithParticipationInEventDTos>>.Failure(Error.NotFound("404",$"{request.EventsId} not found"));
        }

    }
}
