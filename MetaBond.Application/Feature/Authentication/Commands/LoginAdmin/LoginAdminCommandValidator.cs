using FluentValidation;

namespace MetaBond.Application.Feature.Authentication.Commands.LoginAdmin;

public sealed class LoginAdminCommandValidator : AbstractValidator<LoginAdminCommand>
{
    public LoginAdminCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
    }
}