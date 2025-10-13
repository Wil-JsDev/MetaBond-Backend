using FluentValidation;

namespace MetaBond.Application.Feature.Communities.Commands.Update;

public class UpdateCommunitiesCommandValidator : AbstractValidator<UpdateCommunitiesCommand>
{
    public UpdateCommunitiesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The name is required and cannot be empty or null.")
            .MaximumLength(50).WithMessage("The name must not exceed 50 characters.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("The category is required and cannot be empty or null.")
            .MaximumLength(25).WithMessage("The category must not exceed 25 characters.");
    }
}