using MediatR;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Feature.Notifications.Commands.Create;
using MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Interfaces.Service.SignaIR.Hubs;
using MetaBond.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace MetaBond.Infrastructure.Shared.SignaIR.Hubs;

[Authorize]
public class NotificationHub(
    ILogger<NotificationHub> logger,
    IMediator mediator,
    ICurrentService currentService
) : Hub<INotificationHub>
{
    public override Task OnConnectedAsync()
    {
        logger.LogInformation("Connect user: {ContextUser}", currentService.CurrentId);
        return base.OnConnectedAsync();
    }

    public async Task CreateNotification(NotificationType type)
    {
        var command = new CreateNotificationCommand()
        {
            UserId = currentService.CurrentId,
            Type = type
        };

        await mediator.Send(command, CancellationToken.None);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("Disconnect user: {ContextUser}", currentService.CurrentId);
        return base.OnDisconnectedAsync(exception);
    }
}