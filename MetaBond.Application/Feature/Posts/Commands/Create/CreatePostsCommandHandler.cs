using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Commands.Create
{
    internal sealed class CreatePostsCommandHandler : ICommandHandler<CreatePostsCommand, PostsDTos>
    {
        private readonly IPostsRepository _postsRepository;
        private readonly ILogger<CreatePostsCommandHandler> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public CreatePostsCommandHandler(
            IPostsRepository postsRepository, 
            ILogger<CreatePostsCommandHandler> logger, 
            ICloudinaryService cloudinaryService)
        {
            _postsRepository = postsRepository;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<ResultT<PostsDTos>> Handle(
            CreatePostsCommand request, 
            CancellationToken cancellationToken)
        {
            if (request != null)
            { 
                if (request.ImageFile != null)
                {
                    using var stream = request.ImageFile.OpenReadStream();
                    request.ImageUrl = await _cloudinaryService.UploadImageCloudinaryAsync(
                        stream,
                        request.ImageFile.FileName,
                        cancellationToken);

                }

                Domain.Models.Posts postsModel = new()
                {
                    Id = Guid.NewGuid(),
                    Title = request.Title,
                    Content = request.Content,
                    Image =  request.ImageUrl,
                    CommunitiesId = request.CommunitiesId
                };

                await _postsRepository.CreateAsync(postsModel,cancellationToken);

                _logger.LogInformation("Post created successfully with ID: {PostId}", postsModel.Id);

                PostsDTos postsDtos = new
                (
                    PostsId: postsModel.Id,
                    Title: postsModel.Title,
                    Content: postsModel.Content,
                    ImageUrl: postsModel.Image,
                    CommunitiesId: postsModel.CommunitiesId,
                    CreatedAt: postsModel.CreatedAt
                );

                return ResultT<PostsDTos>.Success(postsDtos);
            }
            _logger.LogError("Request is null. Unable to create post.");

            return ResultT<PostsDTos>.Failure(Error.Failure("400", "Invalid request"));
        }
    }
}
