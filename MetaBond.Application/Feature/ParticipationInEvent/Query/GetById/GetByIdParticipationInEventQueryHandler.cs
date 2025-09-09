using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetById;

internal sealed class GetByIdParticipationInEventQueryHandler(
    IParticipationInEventRepository repository,
    ILogger<GetByIdParticipationInEventQueryHandler> logger)
    : IQueryHandler<GetByIdParticipationInEventQuery, ParticipationInEventDTos>
{
    public async Task<ResultT<ParticipationInEventDTos>> Handle(
        GetByIdParticipationInEventQuery request,
        CancellationToken cancellationToken)
    {
        var participationInEvent = await EntityHelper.GetEntityByIdAsync(
            repository.GetByIdAsync,
            request.ParticipationInEventId,
            "ParticipationInEvent",
            logger
        );

        if (participationInEvent.IsSuccess)
        {
            var inEventDTos = ParticipationInEventMapper.ParticipationInEventToDto(participationInEvent.Value);

            logger.LogInformation("Successfully retrieved participation with ParticipationId: {ParticipationId}.",
                participationInEvent.Value.Id);

            return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
        }

        logger.LogError("Participation with ParticipationId: {ParticipationId} not found.",
            request.ParticipationInEventId);

        return ResultT<ParticipationInEventDTos>.Failure(Error.NotFound("404",
            $"{request.ParticipationInEventId} Participation not found"));
    }
}