using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Update
{
    internal sealed class UpdateParticipationInEventCommandHandler : ICommandHandler<UpdateParticipationInEventCommand, ParticipationInEventDTos>
    {
        private readonly IParticipationInEventRepository _participationInEventRepository;
        private readonly ILogger<UpdateParticipationInEventCommandHandler> _logger;

        public UpdateParticipationInEventCommandHandler(
            IParticipationInEventRepository participationInEventRepository, 
            ILogger<UpdateParticipationInEventCommandHandler> logger)
        {
            _participationInEventRepository = participationInEventRepository;
            _logger = logger;
        }

        public async Task<ResultT<ParticipationInEventDTos>> Handle(
            UpdateParticipationInEventCommand request, 
            CancellationToken cancellationToken)
        {
            var participationInEvent = await _participationInEventRepository.GetByIdAsync(request.Id);

            if (participationInEvent != null)
            {
                participationInEvent.EventId = request.EventId;

                await _participationInEventRepository.UpdateAsync(participationInEvent,cancellationToken);

                _logger.LogInformation("Successfully updated participation for ParticipationId: {ParticipationId} with new EventId: {EventId}",
                                        participationInEvent.Id, participationInEvent.EventId);

                ParticipationInEventDTos inEventDTos = new
                (
                    ParticipationInEventId: participationInEvent.Id,
                    EventId: participationInEvent.EventId
                );

                return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
            }

            _logger.LogError("Participation with Id: {ParticipationId} not found for update.", request.Id);

            return ResultT<ParticipationInEventDTos>.Failure(Error.NotFound("404", $"{request.Id} Participation not found"));
        }
    }
}
