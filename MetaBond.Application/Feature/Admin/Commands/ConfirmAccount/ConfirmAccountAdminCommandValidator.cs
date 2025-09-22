using FluentValidation;

namespace MetaBond.Application.Feature.Admin.Commands.ConfirmAccount;

public class ConfirmAccountAdminCommandValidator : AbstractValidator<ConfirmAccountAdminCommand>
{
    public ConfirmAccountAdminCommandValidator()
    {
        RuleFor(x => x.AdminId)
            .NotEmpty().WithMessage("AdminId is required.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .MinimumLength(6).WithMessage("Code must be at least 6 characters long.");
    }
}