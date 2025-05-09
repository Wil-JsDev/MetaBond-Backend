using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Query.GetPostWithAuthor;

internal sealed class GetPostWithAuthorQueryHandler(
    IPostsRepository postsRepository,
    ILogger<GetPostWithAuthorQueryHandler> logger,
    IDistributedCache decoratedCache
    ): IQueryHandler<GetPostWithAuthorQuery, IEnumerable<PostsWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<PostsWithUserDTos>>> Handle(
        GetPostWithAuthorQuery request, 
        CancellationToken cancellationToken)
    {
        if (request != null)
        {
            logger.LogInformation("Processing request to get posts with author info for PostId: {PostId}", request.PostsId);

            var postsWithAuthor = await decoratedCache.GetOrCreateAsync($"GetPostsWithAuthor-{request.PostsId}",
                async () => await postsRepository.GetPostWithAuthorAsync(request.PostsId ?? Guid.Empty, cancellationToken),
                cancellationToken: cancellationToken);

            IEnumerable<Domain.Models.Posts> postsEnumerable = postsWithAuthor.ToList();
            if (!postsEnumerable.Any())
            {
                logger.LogWarning("No posts found with the specified PostId: {PostId}", request.PostsId);

                return ResultT<IEnumerable<PostsWithUserDTos>>.Failure(Error.Failure("400", "No posts found with the given PostId."));
            }

            var postsWithUser = postsEnumerable.Select(x => new PostsWithUserDTos
            (
                PostsId: x.Id,
                Title: x.Title,
                Content: x.Content,
                ImageUrl: x.Image,
                CreatedBy: new UserPostsDTos(
                    UserId: x.CreatedBy!.Id,
                    Username: x.CreatedBy.Username,
                    FirstName: x.CreatedBy.FirstName,
                    LastName: x.CreatedBy.LastName,
                    Photo: x.CreatedBy.Photo
                ),
                CommunitiesId: x.CommunitiesId
            ));

            IEnumerable<PostsWithUserDTos> postsWithUserDTosEnumerable = postsWithUser.ToList();
            
            logger.LogInformation("Successfully retrieved and mapped {Count} posts with author info.", postsWithUserDTosEnumerable.Count());

            return ResultT<IEnumerable<PostsWithUserDTos>>.Success(postsWithUserDTosEnumerable);
        }

        logger.LogWarning("Request object is null. Unable to process post retrieval.");

        return ResultT<IEnumerable<PostsWithUserDTos>>.Failure(Error.NotFound("400", "Request data is missing or invalid."));

    }
}