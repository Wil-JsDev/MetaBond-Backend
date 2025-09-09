using FluentValidation;

namespace MetaBond.Application.Feature.CommunityCategory.Command.Update;

public class UpdateCommunityCategoryValidator : AbstractValidator<UpdateCommunityCategoryCommand>
{
    public UpdateCommunityCategoryValidator()
    {
        RuleFor(x => x.CommunityCategoryId)
            .NotEmpty()
            .WithMessage("CommunityCategoryId is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(30)
            .WithMessage("Name cannot exceed 30 characters.");
    }
}