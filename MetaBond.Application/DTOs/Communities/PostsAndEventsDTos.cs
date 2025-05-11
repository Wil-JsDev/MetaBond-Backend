namespace MetaBond.Application.DTOs.Communities
{
    public sealed record PostsAndEventsDTos
    (
        Guid CommunitieId,
        string? Name,
        string? Category,
        DateTime CreatedAt,
        ICollection<Domain.Models.Posts> Posts,
        ICollection<Domain.Models.Events> Events
    );
}
