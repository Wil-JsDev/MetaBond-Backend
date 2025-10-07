using System.Text.Json.Serialization;

namespace MetaBond.Domain.Models;

public sealed class Events
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Title { get; set; }

    public DateTime? DateAndTime { get; set; }

    public DateTime? CreateAt { get; set; } = DateTime.UtcNow;

    public Guid? CommunitiesId { get; set; }

    public Communities? Communities { get; set; }

    [JsonIgnore]
    public ICollection<EventParticipation>? EventParticipations { get; set; } = new List<EventParticipation>();
}