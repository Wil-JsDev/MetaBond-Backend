using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.DTOs.Posts;

public sealed record CreatePostsParameter(
    string Title,
    string Content,
    Guid? CommunitiesId,
    IFormFile ImageFile
);