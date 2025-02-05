using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Querys.GetById
{
    internal sealed class GetByIdPostsQuerysHandler : IQueryHandler<GetByIdPostsQuerys, PostsDTos>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<GetByIdPostsQuerysHandler> _logger;

        public GetByIdPostsQuerysHandler(IPostsRepository postsRepository, ILogger<GetByIdPostsQuerysHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<PostsDTos>> Handle(GetByIdPostsQuerys request, CancellationToken cancellationToken)
        {
            var posts = await _postsRepository.GetByIdAsync(request.PostsId);

            if (posts != null)
            {
                PostsDTos postsDTos = new
                (
                    PostsId: posts.Id,
                    Title: posts.Title,
                    Content: posts.Content,
                    ImageUrl: posts.Image,
                    CommunitiesId: posts.CommunitiesId,
                    CreatedAt: posts.CreatedAt
                );

                _logger.LogInformation("Post retrieved successfully with ID: {PostId}", posts.Id);

                return ResultT<PostsDTos>.Success(postsDTos);
            }
            _logger.LogError("Post with ID: {PostId} not found.", request.PostsId);

            return ResultT<PostsDTos>.Failure(Error.NotFound("404",$"{request.PostsId} not found"));
        }       
    }
}
