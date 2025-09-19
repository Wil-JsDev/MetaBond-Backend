namespace MetaBond.Domain.Models;

public sealed class Admin
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User? User { get; set; }
}