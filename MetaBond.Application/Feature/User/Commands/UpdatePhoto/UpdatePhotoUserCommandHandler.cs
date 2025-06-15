using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Feature.User.Commands.Update;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.UpdatePhoto;

internal sealed class UpdatePhotoUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdatePhotoUserCommandHandler> logger,
    ICloudinaryService cloudinaryService,
    IDistributedCache decoratedCache
    ) : ICommandHandler<UpdatePhotoUserCommand, string>
{
    public async Task<ResultT<string>> Handle(
        UpdatePhotoUserCommand request, 
        CancellationToken cancellationToken)
    {
            var user = await decoratedCache.GetOrCreateAsync($"update-photo-user-{request.UserId}",
                async () => await userRepository.GetByIdAsync(request.UserId),
                cancellationToken: cancellationToken);
            
            if (user == null)
            {
                logger.LogError($"User with id {request.UserId} not found", request.UserId);
                
                return ResultT<string>.Failure(Error.NotFound("404", "User not found"));
            }
            
            if (request.ImageFile == null || request.ImageFile!.Length == 0)
            {
                logger.LogWarning("Missing or empty image file for user {UserId}.", request.UserId);
                
                return ResultT<string>.Failure(Error.Failure("400", "Image file is required"));
            }  
            
            await using var stream = request.ImageFile!.OpenReadStream();
            
            var imageUrl = await cloudinaryService.UploadImageCloudinaryAsync(
                stream,
                request.ImageFile.FileName,
                cancellationToken);

            user.Photo = imageUrl; 
            
            await userRepository.UpdateAsync(user, cancellationToken);
            
            logger.LogInformation("Updated user with id {UserId}.", request.UserId);
            
            return ResultT<string>.Success(user.Photo!);
    }
}