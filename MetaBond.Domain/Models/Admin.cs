namespace MetaBond.Domain.Models;

public sealed class Admin
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Photo { get; set; }

    public string? Password { get; set; }

    public bool IsEmailConfirmed { get; set; } = false;
}