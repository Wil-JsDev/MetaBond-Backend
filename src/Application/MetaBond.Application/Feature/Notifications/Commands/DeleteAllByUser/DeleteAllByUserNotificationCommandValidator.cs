using FluentValidation;

namespace MetaBond.Application.Feature.Notifications.Commands.DeleteAllByUser;

public class DeleteAllByUserNotificationCommandValidator : AbstractValidator<DeleteAllByUserNotificationCommand>
{
    public DeleteAllByUserNotificationCommandValidator()
    {
        RuleFor(nt => nt.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}