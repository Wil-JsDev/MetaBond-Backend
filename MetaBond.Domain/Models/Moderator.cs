namespace MetaBond.Domain.Models;

public sealed class Moderator
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }

    public User? User { get; set; }
    
    public ICollection<ModeratorCommunity>? ModeratorCommunities { get; set; }
}