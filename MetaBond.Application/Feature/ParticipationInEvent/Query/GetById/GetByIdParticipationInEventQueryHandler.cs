using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetById;

internal sealed class GetByIdParticipationInEventQueryHandler(
    IParticipationInEventRepository repository,
    IDistributedCache decoratedCache,
    ILogger<GetByIdParticipationInEventQueryHandler> logger)
    : IQueryHandler<GetByIdParticipationInEventQuery, ParticipationInEventDTos>
{
    public async Task<ResultT<ParticipationInEventDTos>> Handle(
        GetByIdParticipationInEventQuery request, 
        CancellationToken cancellationToken)
    {
        var participationInEvent = await decoratedCache.GetOrCreateAsync(
            $"ParticipationInEvent-{request.ParticipationInEventId}",
            async () => await repository.GetByIdAsync(request.ParticipationInEventId), 
            cancellationToken: cancellationToken);

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