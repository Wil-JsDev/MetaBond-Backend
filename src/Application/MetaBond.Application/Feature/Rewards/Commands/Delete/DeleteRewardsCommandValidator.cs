using FluentValidation;

namespace MetaBond.Application.Feature.Rewards.Commands.Delete;

public class DeleteRewardsCommandValidator : AbstractValidator<DeleteRewardsCommand>
{
    public DeleteRewardsCommandValidator()
    {
        RuleFor(x => x.RewardsId)
            .NotEmpty().WithMessage("The ID is required and cannot be empty or null.")
            .NotEqual(Guid.Empty).WithMessage("The ID must be a valid GUID.");
    }
}