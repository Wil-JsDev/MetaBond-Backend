using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Commands.Delete;

public class DeleteNotificationCommandValidator : AbstractValidator<DeleteNotificationCommand>
{
    public DeleteNotificationCommandValidator()
    {
        RuleFor(nt => nt.NotificationId)
            .NotEmpty().WithMessage("The notification ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The notification ID must be a valid GUID.");

        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The user ID must be a valid GUID.");
    }
}