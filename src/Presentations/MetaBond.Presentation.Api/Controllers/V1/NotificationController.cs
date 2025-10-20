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
using MetaBond.Application.Feature.Notifications.Query.GetPagedUserId;
using MetaBond.Application.Interfaces.Service;
using MetaBond.Application.Pagination;
using MetaBond.Application.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Swashbuckle.AspNetCore.Annotations;

namespace MetaBond.Presentation.Api.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:ApiVersion}/notifications")]
public class NotificationController(IMediator mediator, ICurrentService currentService) : ControllerBase
{
    [HttpGet("pagination")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<ResultT<PagedResult<NotificationDTos>>> GetPagedNotificationByUserIdAsync(
        [FromQuery] int pageNumber, [FromQuery] int pageSize)
    {
        var query = new GetNotificationsByUserIdPagedQuery()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            UserId = currentService.CurrentId
        };
        return await mediator.Send(query);
    }

    [HttpDelete("me/{notificationId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<Result> DeleteNotificationAsync([FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var command = new DeleteNotificationCommand()
        {
            NotificationId = notificationId,
            UserId = currentService.CurrentId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpDelete("me")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<Result> DeleteAllMyNotificationsAsync(CancellationToken cancellationToken)
    {
        var command = new DeleteAllByUserNotificationCommand()
        {
            UserId = currentService.CurrentId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPut("me/mark-all-as-read")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Mark all my notifications as read",
        Description = "Marks all notifications for the authenticated user as read."
    )]
    public async Task<ResultT<string>> MarkAllMyAsReadAsync(CancellationToken cancellationToken)
    {
        var command = new MarkAllAsReadNotificationCommand()
        {
            UserId = currentService.CurrentId
        };

        return await mediator.Send(command, cancellationToken);
    }

    [HttpPatch("me/{notificationId}/mark-as-read")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Mark my notification as read",
        Description = "Marks a specific notification for the authenticated user as read."
    )]
    public async Task<ResultT<NotificationDTos>> MarkMyAsReadAsync(
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var command = new MarkAsReadNotificationCommand
        {
            NotificationId = notificationId,
            UserId = currentService.CurrentId
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

    [HttpGet("me/next-unread")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get my next unread notification",
        Description = "Retrieves the next unread notification for the authenticated user."
    )]
    public async Task<ResultT<NotificationDTos>> GetMyNextUnreadNotificationAsync(
        CancellationToken cancellationToken)
    {
        var query = new GetNextUnreadNotificationByUserQuery
        {
            UserId = currentService.CurrentId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("me/{notificationId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get my notification by ID",
        Description = "Retrieves a specific notification by its ID, verifying it belongs to the authenticated user."
    )]
    public async Task<ResultT<NotificationWithUserDTos>> GetMyNotificationAsync(
        [FromRoute] Guid notificationId,
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationByUserQuery
        {
            UserId = currentService.CurrentId,
            NotificationId = notificationId
        };

        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("me/recent")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    [SwaggerOperation(
        Summary = "Get my recent notifications",
        Description = "Retrieves a list of recent notifications for the authenticated user."
    )]
    public async Task<ResultT<IEnumerable<NotificationWithUserDTos>>> GetMyRecentNotificationsAsync(
        [FromQuery] int take,
        CancellationToken cancellationToken)
    {
        var query = new GetNotificationRecentByUserQuery
        {
            UserId = currentService.CurrentId,
            Take = take
        };

        return await mediator.Send(query, cancellationToken);
    }
}