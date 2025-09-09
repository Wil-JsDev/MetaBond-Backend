namespace MetaBond.Domain.Models;

public sealed class Interest
{
    public Guid Id { get; set; }

    public Guid? InterestCategoryId { get; set; }

    public string? Name { get; set; }

    public InterestCategory? InterestCategory { get; set; }

    public ICollection<UserInterest>? UserInterests { get; set; }
}