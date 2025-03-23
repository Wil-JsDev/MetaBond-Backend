using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;

internal sealed class UpdateParticipationInEventCommandHandler(
    IParticipationInEventRepository participationInEventRepository,
    ILogger<UpdateParticipationInEventCommandHandler> logger)
    : ICommandHandler<UpdateParticipationInEventCommand, ParticipationInEventDTos>
{
    public async Task<ResultT<ParticipationInEventDTos>> Handle(
        UpdateParticipationInEventCommand request, 
        CancellationToken cancellationToken)
    {
        var participationInEvent = await participationInEventRepository.GetByIdAsync(request.Id);

        if (participationInEvent != null)
        {
            participationInEvent.EventId = request.EventId;

            await participationInEventRepository.UpdateAsync(participationInEvent,cancellationToken);

            logger.LogInformation("Successfully updated participation for ParticipationId: {ParticipationId} with new EventId: {EventId}",
                participationInEvent.Id, participationInEvent.EventId);

            ParticipationInEventDTos inEventDTos = new
            (
                ParticipationInEventId: participationInEvent.Id,
                EventId: participationInEvent.EventId
            );

            return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
        }

        logger.LogError("Participation with Id: {ParticipationId} not found for update.", request.Id);

        return ResultT<ParticipationInEventDTos>.Failure(Error.NotFound("404", $"{request.Id} Participation not found"));
    }
}