namespace MetaBond.Domain.Models;

/// <summary>
/// Represents an individual progress entry within a progress board.
/// Contains details about a specific achievement, task, or update performed by the user.
/// </summary>
public sealed class ProgressEntry
{
    public Guid Id { get; set; }

    public Guid ProgressBoardId { get; set; }

    public ProgressBoard? ProgressBoard { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdateAt { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }
}