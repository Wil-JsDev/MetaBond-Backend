namespace MetaBond.Domain.Models;

public sealed class Notification
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Computed property: returns true if the notification was read
    /// (i.e., if ReadAt has a value assigned).
    /// </summary>
    public bool IsRead => ReadAt.HasValue;

    public DateTime? ReadAt { get; set; }

    // Relationships
    public User? User { get; set; }
}