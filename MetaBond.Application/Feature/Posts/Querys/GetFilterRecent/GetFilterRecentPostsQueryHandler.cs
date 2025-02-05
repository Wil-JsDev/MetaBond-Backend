using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterRecent
{
    internal sealed class GetFilterRecentPostsQueryHandler : IQueryHandler<GetFilterRecentPostsQuery, IEnumerable<PostsDTos>>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetFilterRecentPostsQueryHandler> _logger;

        public GetFilterRecentPostsQueryHandler(
            IPostsRepository postsRepository, 
            ILogger<GetFilterRecentPostsQueryHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
            GetFilterRecentPostsQuery request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var filter = await _postsRepository.FilterRecentPostsByCountAsync(request.TopCount,cancellationToken);
                if (!filter.Any())
                {
                    _logger.LogError("No recent posts found with the specified count: {TopCount}", request.TopCount);

                    return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<PostsDTos> postsDTos = filter.Select(x => new PostsDTos
                (
                    PostsId: x.Id,
                    Title: x.Title,
                    Content: x.Content,
                    ImageUrl: x.Image,
                    CommunitiesId: x.CommunitiesId,
                    CreatedAt: x.CreatedAt
                ));


                _logger.LogInformation("Successfully retrieved {Count} recent posts.", postsDTos.Count());

                return ResultT<IEnumerable<PostsDTos>>.Success(postsDTos);
            }
            _logger.LogError("Received a null request in GetFilterRecentPostsQuery handler.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
