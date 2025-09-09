namespace MetaBond.Domain.Models;

public class CommunityMembership
{
    public Guid UserId { get; set; }

    public Guid CommunityId { get; set; }

    public User? User { get; set; }

    public Communities? Community { get; set; }

    public string? Role { get; set; }

    public bool IsActive { get; set; } = true; // remains a member

    public DateTime? LeftOnUtc { get; set; } // when it came out
}