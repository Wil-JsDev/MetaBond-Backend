namespace MetaBond.Application.DTOs.Communities;

public sealed record PostsAndEventsDTos
(
    Guid CommunitiesId,
    string? Name,
    DateTime CreatedAt,
    IEnumerable<Domain.Models.Posts>? Posts,
    IEnumerable<Domain.Models.Events>? Events
);