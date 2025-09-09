using FluentValidation;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Create;

public class CreateCommunityCategoryValidator : AbstractValidator<CreateCommunityCategoryCommand>
{
    public CreateCommunityCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The category name is required.")
            .MaximumLength(30).WithMessage("The category name must not exceed 30 characters.");
    }
}