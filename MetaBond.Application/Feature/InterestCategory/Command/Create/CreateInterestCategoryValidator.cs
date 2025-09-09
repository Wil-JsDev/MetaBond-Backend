using FluentValidation;

namespace MetaBond.Application.Feature.InterestCategory.Command.Create;

public class CreateInterestCategoryValidator : AbstractValidator<CreateInterestCategoryCommand>
{
    public CreateInterestCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
    }
}