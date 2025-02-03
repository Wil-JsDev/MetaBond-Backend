using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetFilterTop10
{
    internal sealed class GetFilterTop10QueryHandler : IQueryHandler<GetFilterTop10Query, IEnumerable<PostsDTos>>
    {

        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetFilterTop10QueryHandler> _logger;

        public GetFilterTop10QueryHandler(
            IPostsRepository postsRepository, 
            ILogger<GetFilterTop10QueryHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<IEnumerable<PostsDTos>>> Handle(
            GetFilterTop10Query request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            {
                var top10Count = await _postsRepository.FilterTop10RecentPostsAsync(cancellationToken);
                if (!top10Count.Any())
                {
                    _logger.LogError("No posts available in the top 10 recent posts list.");

                    return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "The list is empty"));
                }

                IEnumerable<PostsDTos> postsDTos = top10Count.Select(x => new PostsDTos
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
            _logger.LogError("Received a null or malformed request for fetching the top 10 recent posts.");

            return ResultT<IEnumerable<PostsDTos>>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
