using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Query.GetNotificationRecentByUser;

internal sealed class GetNotificationRecentByUserQueryHandler(
    ILogger<GetNotificationRecentByUserQueryHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetNotificationRecentByUserQuery, IEnumerable<NotificationWithUserDTos>>
{
    public async Task<ResultT<IEnumerable<NotificationWithUserDTos>>> Handle(GetNotificationRecentByUserQuery request,
        CancellationToken cancellationToken)
    {
        if (request.UserId is null || request.UserId == Guid.Empty)
        {
            logger.LogWarning("GetRecentNotificationsByUserQueryHandler: UserId is required.");

            return ResultT<IEnumerable<NotificationWithUserDTos>>.Failure(
                Error.Failure("400", "UserId is required"));
        }

        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        var take = request.Take ?? 10;
        if (take <= 0)
        {
            logger.LogWarning("GetRecentNotificationsByUserQueryHandler: Take must be greater than 0.");
            return ResultT<IEnumerable<NotificationWithUserDTos>>.Failure(
                Error.Failure("400", "Take must be greater than 0."));
        }

        string key = $"notifications-recent-user-{request.UserId}--{request.Take}";
        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var notifications = await notificationRepository.GetRecentNotificationsByUserIdAsync(
                request.UserId ?? Guid.Empty, take, cancellationToken);

            var notificationDtos = notifications.Select(NotificationMapper.ToNotificationWithUserDto).ToList();

            return notificationDtos;
        }, cancellationToken: cancellationToken);

        if (!result.Any())
        {
            logger.LogWarning(
                "GetRecentNotificationsByUserQueryHandler: No notifications found for user ID '{UserId}'.",
                request.UserId);

            return ResultT<IEnumerable<NotificationWithUserDTos>>.Failure(Error.NotFound("404",
                "Notifications not found"));
        }

        logger.LogInformation(
            "GetRecentNotificationsByUserQueryHandler: Notifications found for user ID '{UserId}'.",
            request.UserId);

        return ResultT<IEnumerable<NotificationWithUserDTos>>.Success(result);
    }
}