using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Posts;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Posts.Commands.Create;

internal sealed class CreatePostsCommandHandler(
    IPostsRepository postsRepository,
    IUserRepository userRepository,
    ILogger<CreatePostsCommandHandler> logger,
    ICloudinaryService cloudinaryService)
    : ICommandHandler<CreatePostsCommand, PostsDTos>
{
    public async Task<ResultT<PostsDTos>> Handle(
        CreatePostsCommand request,
        CancellationToken cancellationToken)
    {
        string imageUrl = "";
        if (request.ImageFile != null)
        {
            await using var stream = request.ImageFile.OpenReadStream();
            imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);
        }

        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.CreatedById ?? Guid.Empty,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error!;

        Domain.Models.Posts postsModel = new()
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            Image = imageUrl,
            CreatedById = request.CreatedById ?? Guid.Empty,
            CommunitiesId = request.CommunitiesId
        };

        await postsRepository.CreateAsync(postsModel, cancellationToken);

        logger.LogInformation("Post created successfully with ID: {PostId}", postsModel.Id);

        PostsDTos postsDTos = PostsMapper.PostsToDto(postsModel);

        return ResultT<PostsDTos>.Success(postsDTos);
    }
}