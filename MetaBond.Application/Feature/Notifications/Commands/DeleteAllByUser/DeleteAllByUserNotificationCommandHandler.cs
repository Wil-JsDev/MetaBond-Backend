using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Commands.DeleteAllByUser;

internal sealed class DeleteAllByUserNotificationCommandHandler(
    ILogger<DeleteAllByUserNotificationCommandHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository
) : ICommandHandler<DeleteAllByUserNotificationCommand>
{
    public async Task<Result> Handle(DeleteAllByUserNotificationCommand request, CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        await notificationRepository.DeleteAllByUserIdAsync(request.UserId ?? Guid.Empty, cancellationToken);

        logger.LogInformation(
            "DeleteAllByUserNotificationAsync: All notifications deleted successfully for user ID '{UserId}'.",
            request.UserId);

        return Result.Success();
    }
}