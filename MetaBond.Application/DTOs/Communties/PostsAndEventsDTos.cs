﻿using MetaBond.Domain.Models;

namespace MetaBond.Application.DTOs.Communties
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
