using MetaBond.Application.Abstractions.Messaging;
using MetaBond.Application.DTOs.Notifications;
using MetaBond.Application.Helpers;
using MetaBond.Application.Interfaces.Repository;
using MetaBond.Application.Interfaces.Repository.Account;
using MetaBond.Application.Interfaces.Service.SignaIR.Senders;
using MetaBond.Application.Mapper;
using MetaBond.Application.Utils;
using MetaBond.Domain.Models;
using Microsoft.Extensions.Logging;

namespace MetaBond.Application.Feature.Notifications.Commands.Create;

internal sealed class CreateNotificationCommandHandler(
    ILogger<CreateNotificationCommandHandler> logger,
    IUserRepository userRepository,
    INotificationRepository notificationRepository,
    INotificationSender notificationSender
) : ICommandHandler<CreateNotificationCommand, NotificationDTos>
{
    public async Task<ResultT<NotificationDTos>> Handle(CreateNotificationCommand request,
        CancellationToken cancellationToken)
    {
        var user = await EntityHelper.GetEntityByIdAsync(
            userRepository.GetByIdAsync,
            request.UserId ?? Guid.Empty,
            "User",
            logger
        );

        if (!user.IsSuccess) return user.Error;

        Notification notification = new()
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId ?? Guid.Empty,
            Message = NotificationMapper.MessageNotification(user.Value.Username, request.Type) ?? string.Empty,
            Type = request.Type.ToString()
        };

        await notificationRepository.CreateAsync(notification, cancellationToken);

        logger.LogInformation("CreateNotificationAsync: Notification created successfully for user ID '{UserId}'.",
            request.UserId);

        var notificationDto = NotificationMapper.MapNotificationDTos(notification);

        await notificationSender.SendNewNotificationAsync(request.UserId ?? Guid.Empty, notificationDto);

        logger.LogInformation("CreateNotificationAsync: Notification sent successfully for user ID '{UserId}'.",
            request.UserId);

        return ResultT<NotificationDTos>.Success(notificationDto);
    }
}