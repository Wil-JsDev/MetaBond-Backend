using FluentValidation;

namespace MetaBond.Application.Feature.Interest.Commands.Create;

public class CreateInterestValidator : AbstractValidator<CreateInterestCommand>
{
    public CreateInterestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Interest name is required.")
            .MinimumLength(2).WithMessage("Interest name must be at least 2 characters long.")
            .MaximumLength(50).WithMessage("Interest name must not exceed 100 characters.")
            .Matches(@"^[a-zA-Z\s]+$") // only letters and spaces
            .WithMessage("Interest name can only contain letters and spaces.");
    }
}