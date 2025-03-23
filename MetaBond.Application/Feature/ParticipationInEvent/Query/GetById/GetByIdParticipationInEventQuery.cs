using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;

namespace MetaBond.Application.Feature.ParticipationInEvent.Query.GetById;

    public sealed class GetByIdParticipationInEventQuery : IQuery<ParticipationInEventDTos>
    {
        public Guid ParticipationInEventId { get; set; }
    }

