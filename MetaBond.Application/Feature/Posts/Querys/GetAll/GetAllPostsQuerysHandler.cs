using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetAll
{
    internal sealed class GetAllPostsQuerysHandler : IQueryHandler<GetAllPostsQuerys, IEnumerable<PostsDTos>>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetAllPostsQuerysHandler> _logger;

        public GetAllPostsQuerysHandler(IPostsRepository postsRepository, ILogger<GetAllPostsQuerysHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(GetAllPostsQuerys request, CancellationToken cancellationToken)
        {
            var posts = await _postsRepository.GetAll(cancellationToken);
            if ( posts == null || !posts.Any())
            {
                _logger.LogError("No posts found in the database.");

                return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", ""));
            }

            IEnumerable<PostsDTos> postsDTos = posts.Select(x => new PostsDTos
            (
                PostsId: x.Id,
                Title: x.Title,
                Content: x.Content,
                ImageUrl: x.Image,
                CommunitiesId: x.CommunitiesId,
                CreatedAt: x.CreatedAt
            ));

            _logger.LogInformation("Retrieved {Count} posts from the database.", posts.Count());

            return ResultT<IEnumerable<PostsDTos>>.Success(postsDTos);
        }
    }
}
