using FluentValidation;

namespace MetaBond.Application.Feature.Rewards.Commands.Create;

public class CreateRewardsCommandValidator : AbstractValidator<CreateRewardsCommand>
{
    public CreateRewardsCommandValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");

        RuleFor(x => x.PointAwarded)
            .NotEmpty().WithMessage("The awarded points are required and cannot be empty or null.")
            .GreaterThan(0).WithMessage("The awarded points must be greater than zero.");
    }
}