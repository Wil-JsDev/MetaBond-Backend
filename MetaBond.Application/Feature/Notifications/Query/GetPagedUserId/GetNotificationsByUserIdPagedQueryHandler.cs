using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Mapper;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Serilog;

namespace MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;

internal sealed class GetNotificationsByUserIdPagedQueryHandler(
    ILogger<GetNotificationsByUserIdPagedQueryHandler> logger,
    INotificationRepository notificationRepository,
    IUserRepository userRepository,
    IDistributedCache cache
) : IQueryHandler<GetNotificationsByUserIdPagedQuery, PagedResult<NotificationWithUserDTos>>
{
    public async Task<ResultT<PagedResult<NotificationWithUserDTos>>> Handle(GetNotificationsByUserIdPagedQuery request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger);

        if (!user.IsSuccess) return user.Error;

        var paginationValidation = PaginationHelper.ValidatePagination<NotificationWithUserDTos>(request.PageNumber,
            request.PageSize, logger);

        if (!paginationValidation.IsSuccess) return paginationValidation.Error;

        string key = $"notifications-by-user-id-{request.UserId}-page-{request.PageNumber}-size-{request.PageSize}";

        var result = await cache.GetOrCreateAsync(key, async () =>
        {
            var paged = await notificationRepository.GetPagedNotificationsUserIdAsync(request.UserId ?? Guid.Empty,
                request.PageNumber, request.PageSize, cancellationToken);

            var items = paged.Items.ToList();

            var pagedDto = items.Select(NotificationMapper.ToNotificationWithUserDto).ToList();

            PagedResult<NotificationWithUserDTos> pagedResult = new()
            {
                CurrentPage = paged.CurrentPage,
                Items = pagedDto,
                TotalItems = paged.TotalItems,
                TotalPages = paged.TotalPages
            };

            return pagedResult;
        }, cancellationToken: cancellationToken);

        if (!result.Items.Any())
        {
            logger.LogWarning(
                "GetNotificationsByUserIdPagedQueryHandler: No notifications found for user ID '{UserId}'.",
                request.UserId);

            return ResultT<PagedResult<NotificationWithUserDTos>>.Failure(Error.NotFound("404",
                "Notifications not found"));
        }

        logger.LogInformation(
            "GetNotificationsByUserIdPagedQueryHandler: Notifications found for user ID '{UserId}'.",
            request.UserId);

        return ResultT<PagedResult<NotificationWithUserDTos>>.Success(result);
    }
}