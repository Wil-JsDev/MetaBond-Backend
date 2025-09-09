using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Account.User;
using MetaBond.Application.Helpers;
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
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId,
            "User",
            logger
        );

        if (!user.IsSuccess)
            return user.Error!;

        var isEmailInUse = await userRepository.IsEmailInUseAsync(request.Email!, request.UserId, cancellationToken);
        if (isEmailInUse)
        {
            logger.LogWarning("Email '{Email}' is already in use by another user. Update aborted for User ID {UserId}.",
                request.Email, request.UserId);

            return ResultT<UpdateUserDTos>.Failure(Error.Failure("400", "Email already in use"));
        }

        var isUsernameInUse =
            await userRepository.IsUsernameInUseAsync(request.Username!, request.UserId, cancellationToken);
        if (isUsernameInUse)
        {
            logger.LogWarning(
                "Username '{Username}' is already in use by another user. Update aborted for User ID {UserId}.",
                request.Username, request.UserId);

            return ResultT<UpdateUserDTos>.Failure(Error.Failure("400", "Username already in use"));
        }

        user.Value.Username = request.Username;
        user.Value.Email = request.Email;

        await userRepository.UpdateAsync(user.Value, cancellationToken);

        UpdateUserDTos updateUserDTos = new(
            UserId: user.Value.Id,
            Email: user.Value.Email!,
            Username: request.Username!);

        logger.LogInformation(
            "User with ID {UserId} successfully updated. New username: {Username}, new email: {Email}.",
            user.Value.Id, user.Value.Username, user.Value.Email);

        return ResultT<UpdateUserDTos>.Success(updateUserDTos);
    }
}