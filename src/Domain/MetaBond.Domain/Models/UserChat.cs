namespace MetaBond.Domain.Models;

public sealed class UserChat
{
    public Guid UserId { get; set; }

    public Guid ChatId { get; set; }

    public bool IsMuted { get; set; } = false;

    public bool IsPinned { get; set; } = false;

    public bool IsArchived { get; set; } = false;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public User? User { get; set; }

    public Chat? Chat { get; set; }
}