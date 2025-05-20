namespace MetaBond.Domain.Models;

public sealed class UserInterest
{
    public Guid UserId { get; set; }
    
    public User? User { get; set; }

    public Guid InterestId { get; set; }
    
    public Interest? Interest { get; set; }
}