
using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Create
{
    public sealed class CreateParticipationInEventCommand : ICommand<ParticipationInEventDTos>
    {
        public Guid EventId { get; set; }
    }
}
