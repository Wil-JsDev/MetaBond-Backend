using FluentValidation;

namespace MetaBond.Application.Feature.Authentication.Commands.RefreshTokenUser;

public class RefreshTokenUserCommandValidator : AbstractValidator<RefreshTokenUserCommand>
{
    public RefreshTokenUserCommandValidator()
    {
        RuleFor(us => us.RefreshToken)
            .NotEmpty().WithMessage("RefreshToken is required");
    }
}