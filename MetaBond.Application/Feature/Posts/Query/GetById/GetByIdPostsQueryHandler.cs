using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetById;

internal sealed class GetByIdPostsQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetByIdPostsQueryHandler> logger)
    : IQueryHandler<GetByIdPostsQuery, PostsDTos>
{
    public async Task<ResultT<PostsDTos>> Handle(GetByIdPostsQuery request, CancellationToken cancellationToken)
    {
        var posts = await postsRepository.GetByIdAsync(request.PostsId);
        if (posts != null)
        {
            PostsDTos postsDTos = PostsMapper.PostsToDto(posts);

            logger.LogInformation("Post retrieved successfully with ID: {PostId}", posts.Id);

            return ResultT<PostsDTos>.Success(postsDTos);
        }

        logger.LogError("Post with ID: {PostId} not found.", request.PostsId);

        return ResultT<PostsDTos>.Failure(Error.NotFound("404", $"{request.PostsId} not found"));
    }
}