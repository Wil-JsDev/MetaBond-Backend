using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterTitle
{
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
                if (!postsWithTitle.Any())
                {
                    logger.LogError("No posts found with the title '{Title}'.", request.Title);

                    return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<PostsDTos> postsDTos = postsWithTitle.Select(x => new PostsDTos
                (
                    PostsId: x.Id,
                    Title: x.Title,
                    Content: x.Content,
                    ImageUrl: x.Image,
                    CommunitiesId: x.CommunitiesId,
                    CreatedAt: x.CreatedAt
                ));

                logger.LogInformation("Successfully retrieved {Count} posts with the title '{Title}'.", postsDTos.Count(), request.Title);

                return ResultT<IEnumerable<PostsDTos>>.Success(postsDTos);
            }
            logger.LogError("Invalid request: Title parameter is missing.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
