using FluentValidation;

namespace MetaBond.Application.Feature.Admin.Commands.UnbanUser;

public class UnBanUserCommandValidator : AbstractValidator<UnBanUserCommand>
{
    public UnBanUserCommandValidator()
    {
        RuleFor(us => us.UserId)
            .NotEmpty().WithMessage("The user ID is required and cannot be empty or null.");
    }
}