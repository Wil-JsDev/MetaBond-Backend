using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.ForgotPassword;

public  class ForgotPasswordUserCommandValidator : AbstractValidator<ForgotPasswordUserCommand>
{
    public ForgotPasswordUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");
    }
}