using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.ParticipationInEvent;

namespace MetaBond.Application.Feature.ParticipationInEvent.Commands.Update;

public sealed class UpdateParticipationInEventCommand : ICommand<ParticipationInEventDTos>
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }
}