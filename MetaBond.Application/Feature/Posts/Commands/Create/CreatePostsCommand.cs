using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using Microsoft.AspNetCore.Http;

namespace MetaBond.Application.Feature.Posts.Commands.Create;

public sealed class CreatePostsCommand : ICommand<PostsDTos>
{
    public string? Title { get; set; }

    public string? Content { get; set; }
        
    public Guid? CommunitiesId { get; set; }

    public IFormFile? ImageFile { get; init; }
    
    public Guid? CreatedById { get; set; }
}