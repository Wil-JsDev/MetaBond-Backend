using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.User.Commands.Update;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    ILogger<UpdateUserCommandHandler> logger,
    IDistributedCache decoratedCache
    ) : ICommandHandler<UpdateUserCommand, UpdateUserDTos>
{
    public async Task<ResultT<UpdateUserDTos>> Handle(
        UpdateUserCommand request, 
        CancellationToken cancellationToken)
    {
        
        var user = await decoratedCache.GetOrCreateAsync(
            $"update-user-{request.UserId}",
            async () => await userRepository.GetByIdAsync(request.UserId),
            cancellationToken: cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User with ID {UserId} was not found during update request.", request.UserId);
            
            return ResultT<UpdateUserDTos>.Failure(Error.NotFound("404", "User not found"));
        }

        var isEmailInUse = await userRepository.IsEmailInUseAsync(request.Email!, request.UserId, cancellationToken);
        if (isEmailInUse)
        {
            logger.LogWarning("Email '{Email}' is already in use by another user. Update aborted for User ID {UserId}.", 
                request.Email, request.UserId);
            
            return ResultT<UpdateUserDTos>.Failure(Error.Failure("400", "Email already in use"));
        }

        var isUsernameInUse = await userRepository.IsUsernameInUseAsync(request.Username!, request.UserId, cancellationToken);
        if (isUsernameInUse)
        {
            logger.LogWarning("Username '{Username}' is already in use by another user. Update aborted for User ID {UserId}.", 
                request.Username, request.UserId);
            
            return ResultT<UpdateUserDTos>.Failure(Error.Failure("400", "Username already in use"));
        }

        user.Username = request.Username;
        user.Email = request.Email;

        await userRepository.UpdateAsync(user, cancellationToken);

        UpdateUserDTos updateUserDTos = new(
            UserId:user.Id, 
            Email:user.Email!, 
            Username:request.Username!);
        
        logger.LogInformation("User with ID {UserId} successfully updated. New username: {Username}, new email: {Email}.", 
            user.Id, user.Username, user.Email);

        return ResultT<UpdateUserDTos>.Success(updateUserDTos);
    }
}