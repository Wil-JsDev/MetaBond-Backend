using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterRecent;

internal sealed class GetFilterRecentPostsQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetFilterRecentPostsQueryHandler> logger)
    : IQueryHandler<GetFilterRecentPostsQuery, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterRecentPostsQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var filter = await postsRepository.FilterRecentPostsByCountAsync(request.CommunitiesId,request.TopCount,cancellationToken);
            IEnumerable<Domain.Models.Posts> postsEnumerable = filter.ToList();
            if (!postsEnumerable.Any())
            {
                logger.LogError("No recent posts found with the specified count: {TopCount}", request.TopCount);

                return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
            }

            IEnumerable<PostsDTos> postsDTos = postsEnumerable.Select(x => new PostsDTos
            (
                PostsId: x.Id,
                Title: x.Title,
                Content: x.Content,
                ImageUrl: x.Image,
                CommunitiesId: x.CommunitiesId,
                CreatedAt: x.CreatedAt
            ));


            IEnumerable<PostsDTos> postsDTosEnumerable = postsDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} recent posts.", postsDTosEnumerable.Count());

            return ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);
        }
        logger.LogError("Received a null request in GetFilterRecentPostsQuery handler.");

        return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
    }
}