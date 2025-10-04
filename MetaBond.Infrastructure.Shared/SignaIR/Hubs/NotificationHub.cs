using MediatR;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;
using MetaBond.Application.Interfaces.Service.SignaIR.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MetaBond.Infrastructure.Shared.SignaIR.Hubs;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger,
    IMediator mediator
) : Hub<INotificationHub>
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("Connect user: {ContextUser}", Context.UserIdentifier);
        return base.OnConnectedAsync();
    }


    public async Task GetPaginatedNotifications(int pageNumber, int pageSize)
    {
        var query = new GetNotificationsByUserIdPagedQuery
        {
            UserId = Guid.Parse(Context.UserIdentifier ?? Guid.Empty.ToString()),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await mediator.Send(query);

        if (result.IsSuccess)
        {
            await Clients.Caller.OnNotificationsReceived(result.Value.Items ?? Array.Empty<NotificationDTos>());
        }

        logger.LogError(
            "Error fetching paginated notifications for user {UserId}: {Error}",
            Context.UserIdentifier, result.Error
        );
    }


    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Disconnect user: {ContextUser}", Context.UserIdentifier);
        return base.OnDisconnectedAsync(exception);
    }
}