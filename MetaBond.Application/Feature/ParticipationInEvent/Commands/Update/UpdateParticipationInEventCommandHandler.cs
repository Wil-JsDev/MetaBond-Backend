using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
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
        var participationInEvent = await EntityHelper.GetEntityByIdAsync(
            participationInEventRepository.GetByIdAsync,
            request.Id,
            "ParticipationInEvent",
            logger
        );

        if (!participationInEvent.IsSuccess) return participationInEvent.Error!;
        participationInEvent.Value.EventId = request.EventId;

        await participationInEventRepository.UpdateAsync(participationInEvent.Value, cancellationToken);

        logger.LogInformation(
            "Successfully updated participation for ParticipationId: {ParticipationId} with new EventId: {EventId}",
            participationInEvent.Value.Id, participationInEvent.Value.EventId);

        var inEventDTos = ParticipationInEventMapper.ParticipationInEventToDto(participationInEvent.Value);

        return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
    }
}