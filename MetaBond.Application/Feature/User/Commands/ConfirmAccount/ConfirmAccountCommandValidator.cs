using FluentValidation;

namespace MetaBond.Application.Feature.User.Commands.ConfirmAccount;

public class ConfirmAccountCommandValidator : AbstractValidator<ConfirmAccountCommand>
{
    public ConfirmAccountCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty()
            .WithMessage("Code is required");
        
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");
    }
}