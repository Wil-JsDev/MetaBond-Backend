using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAsRead;

public class MarkAsReadNotificationCommandValidator : AbstractValidator<MarkAsReadNotificationCommand>
{
    public MarkAsReadNotificationCommandValidator()
    {
        RuleFor(us => us.NotificationId)
            .NotEmpty().WithMessage("The notification ID is required and cannot be empty or null.");

        RuleFor(us => us.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.");
    }
}