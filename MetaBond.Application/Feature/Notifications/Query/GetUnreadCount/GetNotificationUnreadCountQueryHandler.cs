using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Base;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Notifications.Query.GetUnreadCount;

internal sealed class GetNotificationUnreadCountQueryHandler(
    ILogger<GetNotificationUnreadCountQueryHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository
) : IQueryHandler<GetNotificationUnreadCountQuery, CountResponse>
{
    public async Task<ResultT<CountResponse>> Handle(GetNotificationUnreadCountQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        var unreadCount =
            await notificationRepository.GetUnreadCountByUserIdAsync(request.UserId ?? Guid.Empty, cancellationToken);

        logger.LogInformation(
            "GetNotificationUnreadCountQueryHandler: Unread notification count retrieved for user ID '{UserId}'.",
            request.UserId);


        return ResultT<CountResponse>.Success(new CountResponse(unreadCount));
    }
}