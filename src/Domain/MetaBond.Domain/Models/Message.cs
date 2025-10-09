namespace MetaBond.Domain.Models;

public sealed class Message
{
    public Guid Id { get; set; }
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; }

    public string? Content { get; set; }
    public DateTime? SentAt { get; set; } = DateTime.UtcNow;

    public bool IsEdited { get; set; } = false;
    public DateTime? EditedAt { get; set; }

    public bool IsDeleted { get; set; } = false;

    // Relationships
    public Chat? Chat { get; set; }
    public User? Sender { get; set; }

    public ICollection<MessageRead> MessageReads { get; set; } = new List<MessageRead>();
}