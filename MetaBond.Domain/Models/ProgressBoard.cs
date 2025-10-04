namespace MetaBond.Domain.Models;

/// <summary>
/// Represents a personal progress board for a user within a community. 
/// Contains multiple progress entries tracking the user's achievements or updates.
/// </summary>
public sealed class ProgressBoard
{
    public Guid Id { get; set; }

    public Guid CommunitiesId { get; set; }

    public Communities? Communities { get; set; }

    public ICollection<ProgressEntry>? ProgressEntries { get; set; }

    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }
}