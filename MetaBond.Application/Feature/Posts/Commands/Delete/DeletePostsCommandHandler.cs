using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Commands.Delete
{
    internal sealed class DeletePostsCommandHandler : ICommandHandler<DeletePostsCommand, Guid>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<DeletePostsCommandHandler> _logger;

        public DeletePostsCommandHandler(IPostsRepository postsRepository, ILogger<DeletePostsCommandHandler> logger)
        {
            _postsRepository = postsRepository;
            _logger = logger;
        }

        public async Task<ResultT<Guid>> Handle(DeletePostsCommand request, CancellationToken cancellationToken)
        {
            var posts = await _postsRepository.GetByIdAsync(request.PostsId);
            if (posts == null)
            {
                _logger.LogError("");

                return ResultT<Guid>.Failure(Error.NotFound("404", $"{request.PostsId} not found"));
            }

            await _postsRepository.DeleteAsync(posts, cancellationToken);

            _logger.LogInformation("");

            return ResultT<Guid>.Success(posts.Id);
        }
    }
}
