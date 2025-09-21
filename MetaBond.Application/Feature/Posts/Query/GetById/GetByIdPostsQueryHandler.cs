using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
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
        var posts = await EntityHelper.GetEntityByIdAsync(
            postsRepository.GetByIdAsync,
            request.PostsId,
            "Posts",
            logger);
        if (!posts.IsSuccess) return posts.Error!;

        var postsDTos = PostsMapper.PostsToDto(posts.Value);

        logger.LogInformation("Post retrieved successfully with ID: {PostId}", posts.Value.Id);

        return ResultT<PostsDTos>.Success(postsDTos);
    }
}