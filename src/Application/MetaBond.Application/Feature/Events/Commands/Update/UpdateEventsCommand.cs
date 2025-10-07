using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Events;

namespace MetaBond.Application.Feature.Events.Commands.Update;

public sealed class UpdateEventsCommand : ICommand<EventsDto>
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }
}