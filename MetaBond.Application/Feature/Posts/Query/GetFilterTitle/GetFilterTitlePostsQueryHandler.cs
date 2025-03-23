using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetFilterTitle;

internal sealed class GetFilterTitlePostsQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetFilterTitlePostsQueryHandler> logger)
    : IQueryHandler<GetFilterTitlePostsQuery, IEnumerable<PostsDTos>>
{
    public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
        GetFilterTitlePostsQuery request, 
        CancellationToken cancellationToken)
    {

        if (request.Title != null)
        {
            var exists = await postsRepository.ValidateAsync(x => x.Title == request.Title);
            if (!exists)
            {
                logger.LogError("No post found with the title '{Title}'.", request.Title);
    
                return ResultT<IEnumerable<PostsDTos>>.Failure(Error.NotFound("404", $"No post exists with the title '{request.Title}'.")); 
            }
            var postsWithTitle = await postsRepository.GetFilterByTitleAsync(request.CommunitiesId,request.Title,cancellationToken);
            IEnumerable<Domain.Models.Posts> postsEnumerable = postsWithTitle.ToList();
            if (!postsEnumerable.Any())
            {
                logger.LogError("No posts found with the title '{Title}'.", request.Title);

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
            logger.LogInformation("Successfully retrieved {Count} posts with the title '{Title}'.", postsDTosEnumerable.Count(), request.Title);

            return ResultT<IEnumerable<PostsDTos>>.Success(postsDTosEnumerable);
        }
        logger.LogError("Invalid request: Title parameter is missing.");

        return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
    }
}