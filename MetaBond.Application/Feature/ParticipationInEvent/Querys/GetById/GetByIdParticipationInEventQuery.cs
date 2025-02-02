
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;

namespace MetaBond.Application.Feature.ParticipationInEvent.Querys.GetById
{
    public sealed class GetByIdParticipationInEventQuery : IQuery<ParticipationInEventDTos>
    {
        public Guid ParticipationInEventId { get; set; }
    }
}
