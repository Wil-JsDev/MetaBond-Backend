using FluentValidation;
using MetaBond.Domain;

namespace MetaBond.Application.Feature.Notifications.Commands.Create;

public class CreateNotificationCommandValidator : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");

        RuleFor(x => x.Type)
            .Must(type => Enum.IsDefined(typeof(NotificationType), type))
            .WithMessage("Invalid notification type. Must be one of: Friendship and Message.");
    }
}