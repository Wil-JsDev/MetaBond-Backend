using FluentValidation;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenAdmin;

public class RefreshTokenAdminCommandValidator : AbstractValidator<RefreshTokenAdminCommand>
{
    public RefreshTokenAdminCommandValidator()
    {
        RuleFor(us => us.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required.");
    }
}