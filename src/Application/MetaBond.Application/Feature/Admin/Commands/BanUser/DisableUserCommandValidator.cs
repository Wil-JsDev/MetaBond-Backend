using FluentValidation;

namespace MetaBond.Application.Feature.Admin.Commands.BanUser;

public class DisableUserCommandValidator : AbstractValidator<DisableUserCommand>
{
    public DisableUserCommandValidator()
    {
        RuleFor(us => us.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.");
    }
}