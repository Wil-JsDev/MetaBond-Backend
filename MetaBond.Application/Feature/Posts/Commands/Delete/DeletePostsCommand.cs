using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;

namespace MetaBond.Application.Feature.Posts.Commands.Delete;

public sealed class DeletePostsCommand : ICommand<Guid>
{
    public Guid PostsId { get; set; }
}