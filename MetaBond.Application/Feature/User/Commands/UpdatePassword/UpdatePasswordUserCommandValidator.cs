using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.UpdatePassword;

public sealed class UpdatePasswordUserValidator : AbstractValidator<UpdatePasswordUserCommand>
{
    public UpdatePasswordUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email format is invalid.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty().WithMessage("Password confirmation is required.")
            .Equal(x => x.NewPassword).WithMessage("Password confirmation must match the new password.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Recovery code is required.");
    }
}