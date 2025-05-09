using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Commands.Create;

internal sealed class CreatePostsCommandHandler(
    IPostsRepository postsRepository,
    ILogger<CreatePostsCommandHandler> logger,
    ICloudinaryService cloudinaryService)
    : ICommandHandler<CreatePostsCommand, PostsDTos>
{
    public async Task<ResultT<PostsDTos>> Handle(
        CreatePostsCommand request, 
        CancellationToken cancellationToken)
    {
            
        if (request != null)
        {
            string imageUrl = "";
            if (request.ImageFile != null)
            {
                using var stream = request.ImageFile.OpenReadStream();
                imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                    stream,
                    request.ImageFile.FileName,
                    cancellationToken);
            }

            Domain.Models.Posts postsModel = new()
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                Image =  imageUrl,
                CreatedById = request.CreatedById ?? Guid.Empty,
                CommunitiesId = request.CommunitiesId
            };

            await postsRepository.CreateAsync(postsModel,cancellationToken);

            logger.LogInformation("Post created successfully with ID: {PostId}", postsModel.Id);

            PostsDTos postsDTos = new
            (
                PostsId: postsModel.Id,
                Title: postsModel.Title,
                Content: postsModel.Content,
                ImageUrl: postsModel.Image,
                CreatedById: postsModel.CreatedById,
                CommunitiesId: postsModel.CommunitiesId,
                CreatedAt: postsModel.CreatedAt
            );

            return ResultT<PostsDTos>.Success(postsDTos);
        }
        logger.LogError("Request is null. Unable to create post.");

        return ResultT<PostsDTos>.Failure(Error.Failure("400", "Invalid request"));
    }
}