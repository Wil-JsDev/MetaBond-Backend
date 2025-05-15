namespace MetaBond.Application.DTOs.Communities
{
    public sealed record PostsAndEventsDTos
    (
        Guid CommunitieId,
        string? Name,
        string? Category,
        DateTime CreatedAt,
        IEnumerable<Domain.Models.Posts> Posts,
        IEnumerable<Domain.Models.Events> Events
    );
}
