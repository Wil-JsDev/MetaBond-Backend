using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Commands.MarkAllAsRead;

public class MarkAllAsReadNotificationCommandValidator : AbstractValidator<MarkAllAsReadNotificationCommand>
{
    public MarkAllAsReadNotificationCommandValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}