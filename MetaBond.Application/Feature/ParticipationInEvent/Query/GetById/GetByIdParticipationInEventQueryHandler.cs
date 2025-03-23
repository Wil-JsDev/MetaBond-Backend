using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
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
        var participationInEvent = await repository.GetByIdAsync(request.ParticipationInEventId);

        if (participationInEvent != null)
        {
            ParticipationInEventDTos inEventDTos = new
            (
                ParticipationInEventId: participationInEvent.Id,
                EventId: participationInEvent.EventId
            );

            logger.LogInformation("Successfully retrieved participation with ParticipationId: {ParticipationId}.", participationInEvent.Id);

            return ResultT<ParticipationInEventDTos>.Success(inEventDTos);
        }
        logger.LogError("Participation with ParticipationId: {ParticipationId} not found.", request.ParticipationInEventId);

        return ResultT<ParticipationInEventDTos>.Failure(Error.NotFound("404", $"{request.ParticipationInEventId} Participation not found"));
    }
}