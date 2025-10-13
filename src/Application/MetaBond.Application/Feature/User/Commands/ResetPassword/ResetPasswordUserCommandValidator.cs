using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.ResetPassword;

public class ResetPasswordUserCommandValidator : AbstractValidator<ResetPasswordUserCommand>
{
    public ResetPasswordUserCommandValidator()
    {
        RuleFor(us => us.UserId)
            .NotEmpty().WithMessage("User ID is required.")
            .Must(id => id != Guid.Empty).WithMessage("User ID cannot be an empty GUID.");

        RuleFor(us => us.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(us => us.ConfirmNewPassword)
            .NotEmpty().WithMessage("Confirm password is required.")
            .Equal(us => us.NewPassword).WithMessage("Passwords do not match.");
    }
}