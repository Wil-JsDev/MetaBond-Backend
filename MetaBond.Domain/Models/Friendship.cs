namespace MetaBond.Domain.Models;

public sealed class Friendship
{
    public Guid Id { get; set; }

    // User sending the request
    public Guid? RequesterId { get; set; }

    public User? Requester { get; set; }

    // User receiving the request
    public Guid? AddresseeId { get; set; }

    public User? Addressee { get; set; }

    public Status Status { get; set; }

    public DateTime? CreateAdt { get; set; } = DateTime.UtcNow;
}