namespace MetaBond.Domain.Models;

public sealed class Interest
{
    public Guid Id { get; set; }
    
    public string? Name { get; set; }
    
    public ICollection<UserInterest>? UserInterests { get; set; }
}