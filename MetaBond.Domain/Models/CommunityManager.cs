namespace MetaBond.Domain.Models;

public sealed class CommunityManager
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public Guid CommunityId { get; set; }

    public User? User { get; set; }
    
    public Communities? Community { get; set; }
}