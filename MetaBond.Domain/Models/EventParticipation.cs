namespace MetaBond.Domain.Models
{
    public sealed class EventParticipation
    {
        public Guid EventId { get; set; }

        public Events? Event { get; set; }

        public Guid ParticipationInEventId { get; set; }

        public ParticipationInEvent? ParticipationInEvent { get; set; }
    }
}