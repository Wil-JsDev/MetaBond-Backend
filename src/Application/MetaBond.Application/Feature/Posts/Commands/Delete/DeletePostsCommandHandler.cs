using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Commands.Delete;

internal sealed class DeletePostsCommandHandler(
    IPostsRepository postsRepository,
    ILogger<DeletePostsCommandHandler> logger)
    : ICommandHandler<DeletePostsCommand, Guid>
{
    public async Task<ResultT<Guid>> Handle(DeletePostsCommand request, CancellationToken cancellationToken)
    {
        var posts = await EntityHelper.GetEntityByIdAsync(
            postsRepository.GetByIdAsync,
            request.PostsId,
            "Posts",
            logger
        );
        if (!posts.IsSuccess) return posts.Error!;

        await postsRepository.DeleteAsync(posts.Value, cancellationToken);

        logger.LogInformation("Post with ID {PostId} successfully deleted.", request.PostsId);

        return ResultT<Guid>.Success(posts.Value.Id);
    }
}