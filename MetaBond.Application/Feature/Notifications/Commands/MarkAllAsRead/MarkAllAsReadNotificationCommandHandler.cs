using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAllAsRead;

internal sealed class MarkAllAsReadNotificationCommandHandler(
    ILogger<MarkAllAsReadNotificationCommandHandler> logger,
    IUserRepository userRepository,
    INotificationRepository notificationRepository
) : ICommandHandler<MarkAllAsReadNotificationCommand, string>
{
    public async Task<ResultT<string>> Handle(MarkAllAsReadNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        await notificationRepository.MarkAllAsReadAsync(request.UserId ?? Guid.Empty, cancellationToken);

        logger.LogInformation(
            "MarkAllAsReadNotificationAsync: All notifications marked as read successfully for user ID '{UserId}'.",
            request.UserId);

        return ResultT<string>.Success("All notifications marked as read successfully.");
    }
}