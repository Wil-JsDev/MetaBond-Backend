using FluentValidation;

namespace MetaBond.Application.Feature.Communities.Commands;

public class CreateCommunitiesCommandValidator : AbstractValidator<CreateCommuntiesCommand>
{
    public CreateCommunitiesCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The name must not exceed 50 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("The category is required and cannot be empty or null.")
            .MaximumLength(25).WithMessage("The category must not exceed 25 characters.");

    }
}