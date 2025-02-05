using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterTitle
{
    internal sealed class GetFilterTitlePostsQueryHandler : IQueryHandler<GetFilterTitlePostsQuery, IEnumerable<PostsDTos>>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetFilterTitlePostsQueryHandler> _logger;

        public GetFilterTitlePostsQueryHandler(
            IPostsRepository postsRepository, 
            ILogger<GetFilterTitlePostsQueryHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
            GetFilterTitlePostsQuery request, 
            CancellationToken cancellationToken)
        {

            if (request.Title != null)
            {
                var postsWithTitle = await _postsRepository.GetFilterByTitleAsync(request.Title,cancellationToken);
                if (!postsWithTitle.Any())
                {
                    _logger.LogError("No posts found with the title '{Title}'.", request.Title);

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

                _logger.LogInformation("Successfully retrieved {Count} posts with the title '{Title}'.", postsDTos.Count(), request.Title);

                return ResultT<IEnumerable<PostsDTos>>.Success(postsDTos);
            }
            _logger.LogError("Invalid request: Title parameter is missing.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
