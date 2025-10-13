using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEventDtos;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetById;

public sealed class GetByIdParticipationInEventQuery : IQuery<ParticipationInEventDTos>
{
    public Guid ParticipationInEventId { get; set; }
}