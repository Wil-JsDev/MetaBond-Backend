namespace MetaBond.Domain.Models;

public sealed class Roles
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public ICollection<User>? Users { get; set; }
}