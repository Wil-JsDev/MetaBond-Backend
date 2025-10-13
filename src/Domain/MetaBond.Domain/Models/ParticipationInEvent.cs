namespace MetaBond.Domain.Models;

public sealed class ParticipationInEvent
{
    public Guid Id { get; set; }

    public Guid? EventId { get; set; }

    public ICollection<EventParticipation>? EventParticipations { get; set; }
}