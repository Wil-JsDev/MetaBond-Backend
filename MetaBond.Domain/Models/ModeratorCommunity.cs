namespace MetaBond.Domain.Models;

public sealed class ModeratorCommunity
{
    public Guid ModeratorId { get; set; }
    
    public Guid CommunitiesId { get; set; }

    public Moderator? Moderator { get; set; }
    
    public Communities? Community { get; set; }
}