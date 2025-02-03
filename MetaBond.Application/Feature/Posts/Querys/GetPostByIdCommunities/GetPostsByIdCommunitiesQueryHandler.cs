using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetPostByIdCommunities
{
    internal sealed class GetPostsByIdCommunitiesQueryHandler : IQueryHandler<GetPostsByIdCommunitiesQuery, IEnumerable<PostsWithCommunitiesDTos>>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetPostsByIdCommunitiesQueryHandler> _logger;

        public GetPostsByIdCommunitiesQueryHandler(
            IPostsRepository postsRepository, 
            ILogger<GetPostsByIdCommunitiesQueryHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<PostsWithCommunitiesDTos>>> Handle(
            GetPostsByIdCommunitiesQuery request, 
            CancellationToken cancellationToken)
        {
            var posts = await _postsRepository.GetByIdAsync(request.PostsId);
            if (posts == null)
            {
                _logger.LogError("Post with ID '{PostsId}' not found.", request.PostsId);

                return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(Error.NotFound("404", $"{request.PostsId} not found"));
            }

            var postsWithCommunities = await _postsRepository.GetPostsByIdWithCommunitiesAsync(request.PostsId,cancellationToken);
            if (!postsWithCommunities.Any())
            {
                _logger.LogError("No communities found for post with ID '{PostsId}'.", request.PostsId);

                return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Failure(Error.Failure("400","The list is empty"));
            }

            IEnumerable<PostsWithCommunitiesDTos> postsWithCommunitiesDTos = postsWithCommunities.Select(x => new PostsWithCommunitiesDTos
            (
                    PostsId: x.Id,
                    Title: x.Title,
                    Content: x.Content,
                    ImageUrl: x.Image,
                    CommunitiesId: x.CommunitiesId,
                    Communities: x.Communities,
                    CreatedAt: x.CreatedAt
            ));

            _logger.LogInformation("Successfully retrieved {Count} posts with their associated communities for post ID '{PostsId}'.", 
                                     postsWithCommunitiesDTos.Count(), request.PostsId);

            return ResultT<IEnumerable<PostsWithCommunitiesDTos>>.Success(postsWithCommunitiesDTos);
        }
    }
}
