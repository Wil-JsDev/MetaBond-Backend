namespace MetaBond.Domain.Models;

public sealed class MessageRead
{
    public Guid MessageId { get; set; }

    public Guid UserId { get; set; }

    public DateTime ReadAt { get; set; } = DateTime.UtcNow;

    // Relationships
    public Message? Message { get; set; }

    public User? User { get; set; }
}