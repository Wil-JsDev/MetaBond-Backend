using Asp.Versioning;
using MediatR;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Feature.Notifications.Commands.Create;
using MetaBond.Application.Feature.Notifications.Commands.Delete;
using MetaBond.Application.Feature.Notifications.Commands.DeleteAllByUser;
using MetaBond.Application.Feature.Notifications.Commands.MarkAllAsRead;
using MetaBond.Application.Feature.Notifications.Commands.MarkAsRead;
using MetaBond.Application.Feature.Notifications.Query.GetById;
using MetaBond.Application.Feature.Notifications.Query.GetNextUnread;
using MetaBond.Application.Feature.Notifications.Query.GetNotificationByUser;
using MetaBond.Application.Feature.Notifications.Query.GetNotificationRecentByUser;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/notifications")]
public class NotificationController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<NotificationDTos>> CreateNotificationAsync([FromBody] CreateNotificationCommand command,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(command, cancellationToken);
    }

    [HttpDelete("{notificationId}/users/{userId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<Result> DeleteAllNotificationsAsync([FromRoute] Guid userId,
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteNotificationCommand()
        {
            NotificationId = notificationId,
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpDelete("users/{userId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<Result> DeleteAllNotificationsAsync([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteAllByUserNotificationCommand()
        {
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("users/{userId}/reads")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<string>> MarkAllAsReadAsync([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        return await mediator.Send(new MarkAllAsReadNotificationCommand() { UserId = userId }, cancellationToken);
    }

    [HttpPatch("{notificationId}/users/{userId}/read")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<NotificationDTos>> MarkAsReadAsync([FromRoute] Guid userId,
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var command = new MarkAsReadNotificationCommand
        {
            NotificationId = notificationId,
            UserId = userId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpGet("{notificationId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<NotificationDTos>> GetNotificationByIdAsync([FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var query = new GetByIdNotificationQuery()
        {
            NotificationId = notificationId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("users/{userId}/next-unread")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<NotificationDTos>> NextUnreadNotificationAsync([FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        var query = new GetNextUnreadNotificationByUserQuery
        {
            UserId = userId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{notificationId}/users/{userId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<NotificationWithUserDTos>> GetNotificationByUserAsync([FromRoute] Guid userId,
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationByUserQuery
        {
            UserId = userId,
            NotificationId = notificationId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("users/{userId}/recent")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<IEnumerable<NotificationWithUserDTos>>> GetNotificationRecentByUserAsync(
        [FromRoute] Guid userId,
        [FromQuery] int take,
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationRecentByUserQuery
        {
            UserId = userId,
            Take = take
        };

        return await mediator.Send(query, cancellationToken);
    }
}