using FluentValidation;

namespace MetaBond.Application.Feature.Rewards.Commands.Update;

public class UpdateRewardsCommandValidator : AbstractValidator<UpdateRewardsCommand>
{
    public UpdateRewardsCommandValidator()
    {
        RuleFor(x => x.RewardsId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
        
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("The description is required and cannot be empty or null.")
            .MaximumLength(255).WithMessage("The description must not exceed 255 characters.");

        RuleFor(x => x.PointAwarded)
            .NotEmpty().WithMessage("The awarded points are required and cannot be empty or null.")
            .GreaterThan(0).WithMessage("The awarded points must be greater than zero.");
    }
}