using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Create;

internal sealed class CreateParticipationInEventCommandHandler(
    IParticipationInEventRepository participationInEventRepository,
    IEventParticipationRepository eventParticipationRepository,
    ILogger<CreateParticipationInEventCommandHandler> logger)
    : ICommandHandler<CreateParticipationInEventCommand, ParticipationInEventDTos>
{
    public async Task<ResultT<ParticipationInEventDTos>> Handle(
        CreateParticipationInEventCommand request,
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            Domain.Models.ParticipationInEvent inEvent = new()
            {
                Id = Guid.NewGuid(),
                EventId = request.EventId
            };

            await participationInEventRepository.CreateAsync(inEvent, cancellationToken);

            logger.LogInformation(
                "Participation created for EventId: {EventId} with ParticipationId: {ParticipationId}",
                inEvent.EventId, inEvent.Id);

            Domain.Models.EventParticipation eventParticipation = new()
            {
                EventId = request.EventId,
                ParticipationInEventId = inEvent.Id
            };

            await eventParticipationRepository.CreateAsync(eventParticipation, cancellationToken);

            logger.LogInformation(
                "EventParticipation record created for EventId: {EventId} with ParticipationInEventId: {ParticipationInEventId}",
                eventParticipation.EventId, eventParticipation.ParticipationInEventId);

            var inEventDTos = EventParticipationMapper.EventParticipationToDto(eventParticipation);

            return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
        }

        logger.LogError("Invalid request received for creating participation in event.");

        return ResultT<ParticipationInEventDTos>.Failure(Error.Failure("400", "Request data is invalid"));
    }
}