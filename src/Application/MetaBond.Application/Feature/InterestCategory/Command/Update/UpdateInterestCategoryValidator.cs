using FluentValidation;

namespace MetaBond.Application.Feature.InterestCategory.Command.Update;

internal sealed class UpdateInterestCategoryValidator : AbstractValidator<UpdateInterestCategoryCommand>
{
    public UpdateInterestCategoryValidator()
    {
        RuleFor(x => x.InterestCategoryId)
            .NotEmpty().WithMessage("InterestCategoryId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name must not exceed 100 characters.");
    }
}