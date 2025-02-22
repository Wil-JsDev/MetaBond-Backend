using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.GetEvent
{
    internal sealed class GetEventParticipationInEventIdQueryHandler : IQueryHandler<GetEventParticipationInEventIdQuery, IEnumerable<ParticipationInEventWithEventsDTos>>
    {
        private readonly IParticipationInEventRepository _repository;
        private readonly IEventsRepository _eventsRepository;
        private readonly ILogger<GetEventParticipationInEventIdQueryHandler> _logger;

        public GetEventParticipationInEventIdQueryHandler(
            IParticipationInEventRepository repository, 
            ILogger<GetEventParticipationInEventIdQueryHandler> logger,
            IEventsRepository eventsRepository)
        {
            _repository = repository;
            _logger = logger;
            _eventsRepository = eventsRepository;
        }

        public async Task<ResultT<IEnumerable<ParticipationInEventWithEventsDTos>>> Handle(
            GetEventParticipationInEventIdQuery request, 
            CancellationToken cancellationToken)
        {
            var events = await _eventsRepository.GetByIdAsync(request.EventId);
            if (events != null)
            {
                var participationInEventsWithEvents = await _repository.GetParticipationByEventIdAsync(request.EventId,cancellationToken);
                if (participationInEventsWithEvents == null || !participationInEventsWithEvents.Any())
                {
                    _logger.LogError("No participation records found for EventId: {EventId}", request.EventId);
                    return ResultT<IEnumerable<ParticipationInEventWithEventsDTos>>.Failure(Error.Failure("400", "The List is empty"));
                }

                var participationInEventsDto = participationInEventsWithEvents.Select(x => new ParticipationInEventWithEventsDTos
                (
                    ParticipationInEventId: x.Id,
                    EventId: x.EventId
                ));


                _logger.LogInformation("Successfully retrieved {Count} participation records for EventId: {EventId}", 
                                        participationInEventsDto.Count(), request.EventId);

                return ResultT<IEnumerable<ParticipationInEventWithEventsDTos>>.Success(participationInEventsDto);
            }

            _logger.LogError("Event with ID {EventId} not found", request.EventId);

            return ResultT<IEnumerable<ParticipationInEventWithEventsDTos>>.Failure(Error.NotFound("400", $"{request.EventId} not found"));
        }
    }
}
