using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTop10;

internal sealed class GetFilterTop10QueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetFilterTop10QueryHandler> logger)
    : IQueryHandler<GetFilterTop10Query, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterTop10Query request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            var top10Count = await postsRepository.FilterTop10RecentPostsAsync(request.CommunitiesId,cancellationToken);
            IEnumerable<Domain.Models.Posts> postsEnumerable = top10Count.ToList();
            if (!postsEnumerable.Any())
            {
                logger.LogError("No posts available in the top 10 recent posts list.");

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

            var dTosEnumerable = postsDTos.ToList();
            logger.LogInformation("Successfully retrieved {Count} recent posts.", dTosEnumerable.Count());

            return ResultT<IEnumerable<PostsDTos>>.Success(dTosEnumerable);
        }
        logger.LogError("Received a null or malformed request for fetching the top 10 recent posts.");

        return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
    }
}