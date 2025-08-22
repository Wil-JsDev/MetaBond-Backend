namespace MetaBond.Domain.Models;

public sealed class User
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Photo { get; set; }

    public string? Password { get; set; }

    public bool IsEmailConfirmed { get; set; }

    // Relationships
    public ICollection<Admin>? AdminRoles { get; set; }

    public ICollection<CommunityManager>? CommunityManagerRoles { get; set; }

    public ICollection<EmailConfirmationToken>? EmailConfirmationTokens { get; set; }

    // Interest
    public ICollection<UserInterest>? Interests { get; set; }

    // Friendships
    public ICollection<Friendship>? SentFriendRequests { get; set; } // Como Requester

    public ICollection<Friendship>? ReceivedFriendRequests { get; set; } // Como Addressee

    public ICollection<Posts>? Posts { get; set; }

    public ICollection<ProgressEntry>? ProgressEntries { get; set; }

    public ICollection<Rewards>? Rewards { get; set; }

    public ICollection<ProgressBoard>? ProgressBoards { get; set; }
}