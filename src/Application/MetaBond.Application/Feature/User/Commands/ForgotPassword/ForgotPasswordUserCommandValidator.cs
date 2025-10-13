using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.ForgotPassword;

public class ForgotPasswordUserCommandValidator : AbstractValidator<ForgotPasswordUserCommand>
{
    public ForgotPasswordUserCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty().WithMessage("UserId is required.")
            .Must(id => id != Guid.Empty).WithMessage("UserId cannot be an empty GUID.");
    }
}